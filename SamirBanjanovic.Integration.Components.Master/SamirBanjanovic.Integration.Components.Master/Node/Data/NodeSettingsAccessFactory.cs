using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Components.Master.Node.Data
{
    public static class NodeSettingsAccessFactory
    {
        public static INodeSettingsDataAccess Get()        
            => new JsonNodeSettingsAccessService();
        
    }
}
