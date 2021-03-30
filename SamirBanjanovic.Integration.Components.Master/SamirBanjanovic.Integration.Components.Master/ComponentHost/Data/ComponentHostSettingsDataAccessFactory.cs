using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Components.Master.ComponentHost.Data
{
    public static class ComponentHostSettingsDataAccessFactory
    {
        public static IComponentHostSettingsDataAccess Get()
            => new JsonComponentHostSettingsAccessService();
    }
}
