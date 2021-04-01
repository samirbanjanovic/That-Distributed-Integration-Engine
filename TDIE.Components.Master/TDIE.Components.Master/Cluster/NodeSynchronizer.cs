using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TDIE.Components.Master.Classes;
using TDIE.Components.Master.Cluster.Enums;
using TDIE.Components.Master.ComponentHost.AccessService;
using TDIE.Components.Master.ComponentHost.Data;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Data.Entities;
using TDIE.Components.Master.Node;
using TDIE.Components.Master.Node.AccessService;
using TDIE.Components.Master.Node.AccessService.Classes;
using TDIE.Components.Master.Node.Data;
using TDIE.Components.Master.Node.Data.Entities;

namespace TDIE.Components.Master.Cluster
{
    public class NodeSynchronizer
    {
        public NodeSynchronizer()
        { }

        public async Task<NodeStateEnum> SynchronizeNodeAsync(NodeServer node)
        {
            NodeStateEnum nodeSyncState = NodeStateEnum.FailedToAcquireDistributedLock;

            // sync all node packages...
            // this will also stop all instances that require changing
            await SynchronizeNodePackagesAsync(node);

            await SynchronizeComonentHostInstancesAndPackagesAsync(node);


            nodeSyncState = NodeStateEnum.NodeSynchronizedWithCluster;


            return nodeSyncState;
        }

        private async Task SynchronizeComonentHostInstancesAndPackagesAsync(NodeServer node)
        {
            // get expected instances on node
            // get instances currently on node
            // request to end all instances and start them again 
            // if there's a discrepency
            INodeAccessService nodeAccess = NodeAccessFactory.Get(node);
            IComponentHostSettingsDataAccess componentSettingsDataAccess = ComponentHostSettingsDataAccessFactory.Get();

            IEnumerable<ComponentHostInstanceSettings> requiredComponentHostInstances = await componentSettingsDataAccess.GetComponentInstanceSettingsAsync(node);
            IEnumerable<Package> requiredComponentHostPackages = await componentSettingsDataAccess.GetComponentPackagesAsync(node);


            ComponentHostInstanceSettings primerComponentHost = requiredComponentHostInstances.First();

            // start host process
            string hostPackageName = await NodeSettingsAccessFactory.Get().GetComponentHostPackageName();

            NodeBasicProcessInformation primerHostProcess = await nodeAccess.StartProcessAsync(hostPackageName, (id: 0, args: new Dictionary<string, string> { { "--port", "4999" } }));
            IComponentHostAccessService primerHostProcessAccess = ComponentHostAccessFactory.Get(primerHostProcess.ProcessUri);
            IEnumerable<PackageDetails> remoteComponentHostPackages = await primerHostProcessAccess.GetComponentHostPackageConfigurationAsync();

            try
            {
                requiredComponentHostPackages
                //.AsParallel()
                //.ForAll
                .Select(package =>
                {
                    var remoteInstallation = remoteComponentHostPackages?.Where(x => string.Compare(package.Name, x.PackageName, true) == 0).ToList() ?? default;
                    PackageDetails packageDetails = null;
                    using (var packageStream = File.OpenRead(package.PackagePath))
                    {

                        if (remoteInstallation is null || !remoteInstallation.Any())
                        {// component host doesn't have required package
                            packageDetails = primerHostProcessAccess.UploadComponentHostPackageAsync(packageStream).Result;
                        }
                        else if (!remoteInstallation.Any(x => x.PackageVersion == package.Version))
                        {
                            // check if any instances are running with the loaded package
                            packageDetails = primerHostProcessAccess.UpdateComponentHostPackageAsync(packageStream).Result;
                        }
                    }

                    return packageDetails;
                })
               .ToList();
            }
            finally
            {
                await primerHostProcessAccess.ShutdownComponentHostAsync();
            }
        }

        private async Task SynchronizeNodePackagesAsync(NodeServer node)
        {

            INodeAccessService nodeAccess = NodeAccessFactory.Get(node);
            INodeSettingsDataAccess nodeSettingsDataAccess = NodeSettingsAccessFactory.Get();


            // get current state of node
            IEnumerable<PackageDetails> remoteNodePackages = await nodeAccess.GetNodePackageConfigurationAsync();

            IEnumerable<Package> expectedNodePackages = await nodeSettingsDataAccess.GetPackagesAsync(node);

            // find node packages that required updating
            // or uploading. Discover all packages
            // where the version doesn't match cluster
            // configuration
            IEnumerable<Package> outOfSyncPackages = FindPackageDiff(remoteNodePackages, expectedNodePackages);

            if (outOfSyncPackages?.Count() > 0)
            {
                // stop all  running instances
                (await nodeAccess.GetNodeProcessStatsAsync())
                    .AsParallel()
                    .ForAll(async instance =>
                    {
                        var instanceAccess = ComponentHostAccessFactory.Get(instance.ProcessUri);
                        await instanceAccess.ShutdownComponentHostAsync();
                    });

                // get information on all hosted processes
                var nodePackageInstances = await nodeAccess.GetNodeProcessStatsAsync();

                foreach (var package in outOfSyncPackages)
                {
                    bool isUpdate = false;
                    IEnumerable<NodeProcessInformation> remotePackageInstances = null;

                    // check if remote node has 
                    // information on current package
                    if (isUpdate = remoteNodePackages != null && remoteNodePackages.Any(x => x.PackageName == package.Name))
                    {
                        // find instances of package to be stopped                                
                        remotePackageInstances = nodePackageInstances.Where(x => x.PackageName == package.Name);

                        if (remotePackageInstances.Any())
                        {
                            remotePackageInstances
                                    .AsParallel()
                                    .ForAll(async instance =>
                                    {
                                        // request shut down of each instance
                                        await ComponentHostAccessFactory
                                                .Get(instance.ProcessUri)
                                                .ShutdownComponentHostAsync();
                                    });
                        }
                    }

                    PackageDetails transmittedPackageDetails;
                    using (var packageStream = File.OpenRead(package.PackagePath))
                    {
                        transmittedPackageDetails = isUpdate ?
                                    await nodeAccess.UpdatePackageAsync(packageStream) :
                                    await nodeAccess.UploadPackageAsync(packageStream);
                    }

                    // if there are instances of the specified package 
                    // request node to kill all instances before updating
                }
            }
        }

        private static IEnumerable<Package> FindPackageDiff(IEnumerable<PackageDetails> remotePackages, IEnumerable<Package> expectedPackages)
        {
            if (remotePackages is null)
            {
                return expectedPackages;
            }

            var oos = expectedPackages
                        .Where(x => !(remotePackages.Any(r => string.Compare(r.PackageName, x.Name, true) == 0 && r.PackageVersion == x.Version)))
                        .ToList();

            return oos;
        }
    }
}

