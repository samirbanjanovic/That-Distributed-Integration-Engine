using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.ComponentHost.Models
{
    public class MessagePublisherConfigurationModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string AssemblyPath { get; set; }

        public string FullyQualifiedClassName { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
