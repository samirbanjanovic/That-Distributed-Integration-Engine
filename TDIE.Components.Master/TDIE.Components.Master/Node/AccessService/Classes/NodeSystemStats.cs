using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.Node.AccessService.Classes
{
    public class NodeSystemStats
    {
        public NodeEnvironmentInformation Environment { get; set; }

        public IEnumerable<NodeProcessGroupInformation> PackageInstanceDetails { get; set; }

        public IEnumerable<NodeNetworkInformation> NetworkInterface { get; set; }
    }
}
