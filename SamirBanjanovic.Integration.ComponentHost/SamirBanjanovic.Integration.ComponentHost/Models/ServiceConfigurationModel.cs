using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnTrac.Integration.ComponentHost.Core;

namespace OnTrac.Integration.ComponentHost.Models
{
    public class ServiceConfigurationModel
        : IServiceConfiguration
        , ICloneable
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string AssemblyPath { get; set; }

        public string FullyQualifiedClassName { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public object Clone()
        {
            return new ServiceConfigurationModel
            {
                Version = this.Version,
                Description = this.Description,
                AssemblyPath = this.AssemblyPath,
                FullyQualifiedClassName = this.FullyQualifiedClassName,
                Properties = this.Properties?.ToDictionary(x => x.Key, x => x.Value)                
            };            
        }
    }
}
