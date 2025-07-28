using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.NodeManager.ComponentHost.WebApi;

namespace TDIE.Components.NodeManager.ComponentHost.AccessService
{
    public static class ComponentHostAccessFactory
    {
        private static readonly ConcurrentDictionary<string, IComponentHostAccessService> _accessors = new ConcurrentDictionary<string, IComponentHostAccessService>();

        public static IComponentHostAccessService Get(string uri)
            => _accessors.GetOrAdd(uri, s => new ComponentHostWebApiAccessService(s));
    }
}
