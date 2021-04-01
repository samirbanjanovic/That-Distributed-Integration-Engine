using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.NodeManager.Node.Data
{
    public static class NodeSettingsAccessFactory
    {
        public static INodeSettingsDataAccess Get()        
            => new JsonNodeSettingsAccessService();
        
    }
}
