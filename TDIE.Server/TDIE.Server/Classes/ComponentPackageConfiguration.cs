using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TDIE.Server.Core;

namespace TDIE.Server.Classes
{
    public class ComponentPackageConfiguration
        : IComponentPackageConfiguration
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string ComponentAssembly { get; set; }
        public string FullyQualifiedComponentName { get; set; }
        public IEnumerable<string> RequiredProperties { get; set; }
    }
}
