using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.ComponentHost.Models
{
    public class ServiceConfigurationModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string AssemblyPath { get; set; }

        public string FullyQualifiedClassName { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public MessagePublisherConfigurationModel MessagePublisherConfiguration { get; set; }
    }
}
