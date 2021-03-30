using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Components.Master.Node.AccessService;
using OnTrac.Integration.Components.Master.Node.Data.Entities;
using OnTrac.Integration.Components.Master.Node.WebApi;

namespace OnTrac.Integration.Components.Master.Node
{
    public static class NodeAccessFactory
    {
        private static readonly ConcurrentDictionary<NodeServer, INodeAccessService> _nodeAccessors = new ConcurrentDictionary<NodeServer, INodeAccessService>();

        public static INodeAccessService Get(NodeServer node)
            => _nodeAccessors.GetOrAdd(node, n => new NodeWebApiAccessService(node));
    }
}
