using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.Master.Node.AccessService;
using TDIE.Components.Master.Node.Data.Entities;
using TDIE.Components.Master.Node.WebApi;

namespace TDIE.Components.Master.Node
{
    public static class NodeAccessFactory
    {
        private static readonly ConcurrentDictionary<NodeServer, INodeAccessService> _nodeAccessors = new ConcurrentDictionary<NodeServer, INodeAccessService>();

        public static INodeAccessService Get(NodeServer node)
            => _nodeAccessors.GetOrAdd(node, n => new NodeWebApiAccessService(node));
    }
}
