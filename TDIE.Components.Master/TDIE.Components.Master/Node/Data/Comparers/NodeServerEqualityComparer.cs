using System;
using System.Collections.Generic;
using TDIE.Components.Master.Node.Data.Entities;

namespace TDIE.Components.Master.Node.Data.Comparers
{
    public class NodeServerEqualityComparer
        : EqualityComparer<NodeServer>
    {
        public override bool Equals(NodeServer x, NodeServer y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if(x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public override int GetHashCode(NodeServer obj)
        {
            return (obj.NetworkIp?.Trim()?.ToUpper().GetHashCode() ^ obj.NetworkName?.Trim()?.ToUpper().GetHashCode() ^ obj.NodeApiUri?.Trim()?.ToUpper().GetHashCode()) ?? 0;
        }
    }
}
