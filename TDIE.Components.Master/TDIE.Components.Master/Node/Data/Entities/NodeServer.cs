using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.Node.Data.Entities
{
    public class NodeServer
        : IEquatable<NodeServer>
    {
        public string NetworkName { get; set; }

        public string NetworkIp { get; set; }

        public string NodeApiUri { get; set; }

        public bool Equals(NodeServer other)
        {
            return string.Compare(NetworkName, other.NetworkName, true) == 0
                && string.Compare(NetworkIp, other.NetworkIp, true) == 0
                && string.Compare(NodeApiUri, other.NodeApiUri, true) == 0;
        }
    }
}
