using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.Master.ComponentHost.AccessService;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Node.AccessService.Classes;

namespace TDIE.Components.Master.Cluster.Classes
{
    public class NodeComponent
    {
        //public ComponentInstanceSettings Component { get; set; }
        public NodeBasicProcessInformation ProcessInformation { get; set; }

        public IComponentHostAccessService ComponentHostAccess { get; set; }
    }
}
