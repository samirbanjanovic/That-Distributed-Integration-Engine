using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TDIE.Components.Master.Cluster;
using TDIE.Components.Master.Cluster.Classes;
using TDIE.Components.Master.ComponentHost.Data;
using TDIE.Components.Master.Node;
using TDIE.Components.Master.Node.AccessService.Classes;
using TDIE.Components.Master.Node.Data;
using TDIE.Components.Master.Node.Data.Entities;
using TDIE.Components.Master.WebApi;
using TDIE.Core;
using Timer = System.Timers.Timer;

namespace TDIE.Components.Master
{
    public sealed class MasterComponent
        : IComponent
    {
        private readonly ILogger<MasterComponent> _logger;        
        private readonly NodeSynchronizer _nodeSynchronizer;
        private readonly DistributedLockFactory _distributedLockFactory;
        private readonly IConfiguration _masterConfiguration;
        private readonly Program _masterWebApi;
        private Timer _syncTimer;

        private ClusterManager _clusterManager;

        public MasterComponent(IComponentSettings settings, ILogger<MasterComponent> logger)
        {
            Settings = settings;
            _masterConfiguration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(Settings.Properties["masterConfigurationFile"], optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            _logger = logger;
            
            _nodeSynchronizer = new NodeSynchronizer();
            _distributedLockFactory = new DistributedLockFactory(_masterConfiguration);

            
            _masterWebApi = new Program(_masterConfiguration);
        }


        public IComponentSettings Settings { get; }

        public string Name => Settings.Name;
        public Guid InstanceId { get; } = Guid.NewGuid();
        public ObjectState State { get; }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {            
            IEnumerable<NodeServer> initialNodes = null;
            var initialNodesTask = Task.Run(async () => initialNodes = await NodeSettingsAccessFactory.Get().GetNodesAsync());
            Task.WaitAll(new[] { initialNodesTask }, cancellationToken);

            if (_clusterManager == null)
            {
                _clusterManager = new ClusterManager(initialNodes, _nodeSynchronizer, _distributedLockFactory);
            }

            //_syncTimer = new Timer(double.Parse(_masterConfiguration["Cluster:SyncInterval"] as string ?? "50000"));
            //_syncTimer.Elapsed += async (s, e) => await SyncCluster();
            //_syncTimer.Enabled = true;

            await SyncCluster();

            // add cancelation token logic to start/stop server

            await _masterWebApi.StartServerAsync(cancellationToken);
        }

        private bool _syncStopped = false;

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _syncTimer.Enabled = false;
            _syncTimer.Elapsed -= async (s, e) => await SyncCluster();
            _syncStopped = true;
            return Task.CompletedTask;
        }


        public async Task SyncCluster()
        {
            try
            {
                IEnumerable<NodeServer> registeredNodes = await NodeSettingsAccessFactory.Get().GetNodesAsync();
                IEnumerable<NodeServer> clusterNodes = _clusterManager.NodeComponentDistribution.Keys;

                // remove nodes that are no longer desired
                var nodesToRemove = clusterNodes.Where(x => !registeredNodes.Select(rn => rn.NetworkName).Contains(x.NetworkName));
                if (nodesToRemove.Any())
                {
                    nodesToRemove//.AsParallel()
                                 //.ForAll(async node => await _clusterManager.ShrinkClusterAsync(node));                                
                                .Select(async node => await _clusterManager.ShrinkClusterAsync(node))
                                .ToList();
                }   

                // expand cluster using new nodes
                var newNodes = registeredNodes.Where(x => !clusterNodes.Select(cn => cn.NetworkName).Contains(x.NetworkName));
                if (newNodes.Any())
                {
                    newNodes//.AsParallel()
                            //.ForAll(async node => await _clusterManager.ExpandClusterAsync(node));
                            .Select(async node => await _clusterManager.ExpandClusterAsync(node))
                            .ToList();
                }

                // sync remaining nodes
                var remainingNodes = registeredNodes.Where(x => !newNodes.Select(nc => nc.NetworkName).Contains(x.NetworkName));

                if (remainingNodes.Any())
                {
                    remainingNodes//.AsParallel()
                                  //.ForAll(async node => await _clusterManager.SyncNode(node));
                                  .Select(async node => await _clusterManager.SyncNode(node))
                                  .ToList();
                }
            }
            finally
            {

            }           
        }

        public void Dispose()
        {
            if(!_syncStopped)
            {
                _syncTimer.Enabled = false;
                _syncTimer.Elapsed -= async (s, e) => await SyncCluster();
            }

            _syncTimer.Dispose();
        }
    }
}