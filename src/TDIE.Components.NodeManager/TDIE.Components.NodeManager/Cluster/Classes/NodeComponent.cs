using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.NodeManager.ComponentHost.AccessService;
using TDIE.Components.NodeManager.ComponentHost.Data.Entities;
using TDIE.Components.NodeManager.Node.AccessService.Classes;

namespace TDIE.Components.NodeManager.Cluster.Classes
{
    public class NodeComponent
    {
        //public ComponentInstanceSettings Component { get; set; }
        public NodeBasicProcessInformation ProcessInformation { get; set; }

        public IComponentHostAccessService ComponentHostAccess { get; set; }
    }
}
