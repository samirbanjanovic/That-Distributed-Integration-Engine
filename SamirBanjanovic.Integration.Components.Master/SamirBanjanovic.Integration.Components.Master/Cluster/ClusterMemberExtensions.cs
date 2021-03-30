using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Components.Master.ComponentHost.AccessService;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.Node.AccessService.Classes;
using OnTrac.Integration.Components.Master.Node.Data.Entities;

namespace OnTrac.Integration.Components.Master.Cluster
{
    internal static class ClusterMemberExtensions
    {
        public static string GetDistributedLockName(this ComponentHostInstanceSettingsWithPublisher component, NodeServer node)        
                    => $"{node.NetworkName}.{component.PackageName}.{component.Id}";


        public static IComponentHostAccessService GetComponentHostAccessService(this NodeBasicProcessInformation processInformation)
            => ComponentHostAccessFactory.Get(processInformation.ProcessUri);

    }
}
