using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.Master.ComponentHost.WebApi;

namespace TDIE.Components.Master.ComponentHost.AccessService
{
    public static class ComponentHostAccessFactory
    {
        private static readonly ConcurrentDictionary<string, IComponentHostAccessService> _accessors = new ConcurrentDictionary<string, IComponentHostAccessService>();

        public static IComponentHostAccessService Get(string uri)
            => _accessors.GetOrAdd(uri, s => new ComponentHostWebApiAccessService(s));
    }
}
