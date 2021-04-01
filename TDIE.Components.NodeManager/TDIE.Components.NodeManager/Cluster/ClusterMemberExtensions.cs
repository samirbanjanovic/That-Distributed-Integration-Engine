using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.NodeManager.ComponentHost.AccessService;
using TDIE.Components.NodeManager.ComponentHost.Data.Entities;
using TDIE.Components.NodeManager.Node.AccessService.Classes;
using TDIE.Components.NodeManager.Node.Data.Entities;

namespace TDIE.Components.NodeManager.Cluster
{
    internal static class ClusterMemberExtensions
    {
        public static string GetDistributedLockName(this ComponentHostInstanceSettingsWithPublisher component, NodeServer node)        
                    => $"{node.NetworkName}.{component.PackageName}.{component.Id}";


        public static IComponentHostAccessService GetComponentHostAccessService(this NodeBasicProcessInformation processInformation)
            => ComponentHostAccessFactory.Get(processInformation.ProcessUri);

    }
}
