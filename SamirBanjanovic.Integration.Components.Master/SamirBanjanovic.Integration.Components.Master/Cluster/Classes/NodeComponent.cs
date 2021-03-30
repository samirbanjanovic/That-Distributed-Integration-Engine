using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Components.Master.ComponentHost.AccessService;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.Node.AccessService.Classes;

namespace OnTrac.Integration.Components.Master.Cluster.Classes
{
    public class NodeComponent
    {
        //public ComponentInstanceSettings Component { get; set; }
        public NodeBasicProcessInformation ProcessInformation { get; set; }

        public IComponentHostAccessService ComponentHostAccess { get; set; }
    }
}
