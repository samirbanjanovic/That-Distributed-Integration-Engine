using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TDIE.Components.Master.ComponentHost.AccessService;
using TDIE.Components.Master.ComponentHost.Data;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Data.Entities;
using TDIE.Components.Master.Node;
using TDIE.Components.Master.Node.AccessService;
using TDIE.Components.Master.Node.AccessService.Classes;
using TDIE.Components.Master.Node.Data;
using TDIE.Components.Master.Node.Data.Comparers;
using TDIE.Components.Master.Node.Data.Entities;

namespace TDIE.Components.Master.Cluster
{
    public class ClusterManager
    {
        private ConcurrentDictionary<NodeServer, ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher InstanceDetails, NodeBasicProcessInformation ProcessInfo)>> _nodeComponentDistribution;

        private ConcurrentDictionary<NodeServer, int> _occupiedNodePorts;

        private readonly DistributedLockFactory _distributedLockFactory;
        private readonly NodeSynchronizer _nodeSynchronizer;

        public ClusterManager(IEnumerable<NodeServer> nodes, NodeSynchronizer nodeSynchronizer, DistributedLockFactory distributedLockFactory)
        {
            _nodeComponentDistribution = new ConcurrentDictionary<NodeServer, ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher, NodeBasicProcessInformation)>>();
            _occupiedNodePorts = new ConcurrentDictionary<NodeServer, int>();

            _distributedLockFactory = distributedLockFactory;
            _nodeSynchronizer = nodeSynchronizer;

            PopulateNodeComponentDistribution(nodes);
        }

        public IReadOnlyDictionary<NodeServer, ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher InstanceDetails, NodeBasicProcessInformation ProcessInfo)>> NodeComponentDistribution
            => _nodeComponentDistribution;

        public void ResyncLocalData(IEnumerable<NodeServer> nodes)
        {
            _nodeComponentDistribution = new ConcurrentDictionary<NodeServer, ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher InstanceDetails, NodeBasicProcessInformation ProcessInfo)>>();
            PopulateNodeComponentDistribution(nodes);
        }

        private void PopulateNodeComponentDistribution(IEnumerable<NodeServer> nodes)
        {
            nodes.AsParallel()
                 .ForAll(async node =>
                    {
                        INodeAccessService nodeAccess = NodeAccessFactory.Get(node);
                        IEnumerable<NodeBasicProcessInformation> nodeProcesses = await nodeAccess.GetPackageProcessInstancesAsync();
                        IComponentHostSettingsDataAccess componentHostDataAccess = ComponentHostSettingsDataAccessFactory.Get();
                        IEnumerable<ComponentHostInstanceSettingsWithPublisher> nodeInstances = await componentHostDataAccess.GetComponentInstanceSettingsAsync(node);

                        if (nodeProcesses?.Any() == true)
                        {
                            foreach (var process in nodeProcesses)
                            {
                                var instanceSettings = nodeInstances.FirstOrDefault(x => x.Id == process.SettingsId);
                                if (instanceSettings != null)
                                {
                                    AddOrUpdateComponentDistribution(node, (ComponentInstanceSettings: instanceSettings, ComponentHostProcess: process));
                                }
                            }
                        }                       
                    });
        }

        private void AddOrUpdateComponentDistribution(NodeServer node, (ComponentHostInstanceSettingsWithPublisher ComponentInstanceSettings, NodeBasicProcessInformation ComponentHostProcess) instanceDetails)
        {
            // code for storing instances being started
            _nodeComponentDistribution.AddOrUpdate(node, new ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher, NodeBasicProcessInformation)>(new[] { instanceDetails })
             ,
             (n, bag) =>
             {
                 bag.Add(instanceDetails);
                 return bag;
             });
        }

        public async Task SyncNode(NodeServer node)
        {
            // acquire a node wide lock to ensure we're not stepping 
            // on each others totes
            using (var dLock = _distributedLockFactory.TryAcquireLockAsync(node.NetworkName))
            {
                if (dLock != null)
                {
                    // make sure the node has all proper packages
                    // present for init
                    await _nodeSynchronizer.SynchronizeNodeAsync(node);
                }
            }
        }


        public async Task ExpandClusterAsync(NodeServer node)
        {
            // acquire a node wide lock to ensure we're not stepping 
            // on each others totes
            using (var dLock = _distributedLockFactory.TryAcquireLockAsync(node.NetworkName))
            {
                if (dLock != null)
                {
                    // make sure the node has all proper packages
                    // present for init
                    await _nodeSynchronizer.SynchronizeNodeAsync(node);

                    // start all node procsses 
                    INodeAccessService nodeAccess = NodeAccessFactory.Get(node);

                    // eliminate this code and call "StartComponentHostInstanceAsync"

                    IComponentHostSettingsDataAccess componentHostDataAccess = ComponentHostSettingsDataAccessFactory.Get();                    
                    var componentInstances = await componentHostDataAccess.GetComponentInstanceSettingsAsync();
                    
                    foreach (var instance in componentInstances)
                    {
                        await StartInstanceOnNode(instance, node);
                    }

                    //componentInstances
                    //    .AsParallel()
                    //    .ForAll(async instance => await StartInstanceOnNode(instance, node));
                }
            }
        }

        public async Task ShrinkClusterAsync(NodeServer node)
        {
            using (var dbLock = _distributedLockFactory.TryAcquireLockAsync(node.NetworkName))
            {
                if (dbLock != null)
                {
                    INodeAccessService nodeAccess = NodeAccessFactory.Get(node);

                    var nodeProcessInstances = await nodeAccess.GetPackageProcessInstancesAsync();

                    nodeProcessInstances
                        .AsParallel()
                        .ForAll(async processInstance =>
                        {
                            IComponentHostAccessService processInstanceAccess = processInstance.GetComponentHostAccessService();
                            await processInstanceAccess.StopHostServicesAsync();
                            await processInstanceAccess.ShutdownComponentHostAsync();
                        });
                }
            }
        }

        public async Task StartComponentHostInstanceAsync(ComponentHostInstanceSettingsWithPublisher componentInstanceSettings, IEnumerable<NodeServer> nodes)
        {
            foreach (var node in nodes)
            {
                // acquire distributed lock for specific component instance
                // this way other nodes can publish othe instances
                // problem to consider up the chain -- how to 
                // ensure we're not sending same work twice.  
                // have a last changed field? 
                using (var dLock = await _distributedLockFactory.TryAcquireLockAsync(componentInstanceSettings.GetDistributedLockName(node)))
                {

                    if (dLock != null)
                    {
                        // should we lock the specific node?
                        // this way we ensure that no other node
                        // is attemping to refresh/reset instances
                        await StartInstanceOnNode(componentInstanceSettings, node);                       
                    }
                }
            }
        }

        private async Task StartInstanceOnNode(ComponentHostInstanceSettingsWithPublisher componentInstanceSettings, NodeServer node)
        {
            // start host process
            string componentHostPackageName = await NodeSettingsAccessFactory.Get().GetComponentHostPackageName();
            INodeAccessService nodeAccess = NodeAccessFactory.Get(node);

            // request a new component host that will manage component received 
            // here via "componentInstanceSettings"
            // get and increment port value for component host
            var port = _occupiedNodePorts.AddOrUpdate(node, 5000, (n, p) => p++);
            var componentHostProcess = await nodeAccess.StartProcessAsync(componentHostPackageName, (id: componentInstanceSettings.Id, args: new Dictionary<string, string> { { "--port", port.ToString() } }));

            if (componentHostProcess != null)
            {
                var instanceDetails = (ComponentInstanceSettings: componentInstanceSettings, ComponentHostProcess: componentHostProcess);

                var componentHostAccess = componentHostProcess.GetComponentHostAccessService();

                if (await componentHostAccess.InitializeComponentServiceAsync(componentInstanceSettings.PackageName))
                {
                    bool canStart = true;

                    if (componentInstanceSettings.MessagePublisher != null)
                    {
                        canStart = await componentHostAccess.InitializeMessagePublisherServiceAsync(componentInstanceSettings.MessagePublisher.PackageName);
                    }

                    if (canStart)
                    {
                        await componentHostAccess.StartHostServicesAsync();
                    }

                    AddOrUpdateComponentDistribution(node, instanceDetails);
                }

            }
        }


        public Task StopComponentHostInstanceAsync(IEnumerable<NodeServer> nodes, ComponentHostInstanceSettingsWithPublisher componentInstanceSettings, bool shutdownAfterStop = true)
        {
            // get all active processes from nodes
            nodes.AsParallel()
                 .ForAll(async node =>
                 {
                     using (var dLock = await _distributedLockFactory.TryAcquireLockAsync(componentInstanceSettings.GetDistributedLockName(node)))
                     {
                         if (dLock != null)
                         {
                             if (_nodeComponentDistribution.TryGetValue(node, out ConcurrentBag<(ComponentHostInstanceSettingsWithPublisher InstanceDetails, NodeBasicProcessInformation ProcessInfo)> nodeInstances))
                             {
                                 var processIntanceDetails = nodeInstances.Where(x => x.InstanceDetails.Id == componentInstanceSettings.Id);
                                 if (processIntanceDetails.Any())
                                 {
                                     foreach (var instance in processIntanceDetails)
                                     {
                                         IComponentHostAccessService instanceAccess = instance.ProcessInfo.GetComponentHostAccessService();
                                         await instanceAccess.StopHostServicesAsync();

                                         if (shutdownAfterStop)
                                         {
                                             await instanceAccess.ShutdownComponentHostAsync();
                                             nodeInstances.TryTake(out (ComponentHostInstanceSettingsWithPublisher InstanceDetails, NodeBasicProcessInformation ProcessInfo) removedInstance);
                                         }


                                     }
                                 }
                             }
                         }
                     }
                 });

            return Task.CompletedTask;
        }

        // component instance management

    }
}
