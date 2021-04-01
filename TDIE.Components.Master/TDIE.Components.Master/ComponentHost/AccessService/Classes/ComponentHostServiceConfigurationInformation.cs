using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.ComponentHost.AccessService.Classes
{
    public class ComponentHostServiceConfigurationInformation
    {
        public string Name { get; set; }

        public string Verison { get; set; }

        public string Description { get; set; }

        public string AssemblyPath { get; set; }

        public string FullyQualifiedClassName { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
