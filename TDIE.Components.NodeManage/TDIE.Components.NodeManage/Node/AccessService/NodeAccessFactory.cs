using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.NodeManager.Node.AccessService;
using TDIE.Components.NodeManager.Node.Data.Entities;
using TDIE.Components.NodeManager.Node.WebApi;

namespace TDIE.Components.NodeManager.Node
{
    public static class NodeAccessFactory
    {
        private static readonly ConcurrentDictionary<NodeServer, INodeAccessService> _nodeAccessors = new ConcurrentDictionary<NodeServer, INodeAccessService>();

        public static INodeAccessService Get(NodeServer node)
            => _nodeAccessors.GetOrAdd(node, n => new NodeWebApiAccessService(node));
    }
}
