using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.Master.ComponentHost.AccessService;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Node.AccessService.Classes;
using TDIE.Components.Master.Node.Data.Entities;

namespace TDIE.Components.Master.Cluster
{
    internal static class ClusterMemberExtensions
    {
        public static string GetDistributedLockName(this ComponentHostInstanceSettingsWithPublisher component, NodeServer node)        
                    => $"{node.NetworkName}.{component.PackageName}.{component.Id}";


        public static IComponentHostAccessService GetComponentHostAccessService(this NodeBasicProcessInformation processInformation)
            => ComponentHostAccessFactory.Get(processInformation.ProcessUri);

    }
}
