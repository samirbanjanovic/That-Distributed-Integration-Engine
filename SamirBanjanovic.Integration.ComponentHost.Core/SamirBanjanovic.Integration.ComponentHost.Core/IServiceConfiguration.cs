using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.ComponentHost.Core
{
    public interface IServiceConfiguration
    {
        string Name { get; set; }

        string Version { get; set; }

        string Description { get; set; }

        string AssemblyPath { get; set; }

        string FullyQualifiedClassName { get; set; }

        IDictionary<string, string> Properties { get; set; }
    }
}
