 using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Core
{
    public interface IComponentPackageConfiguration
    {
        string Name { get; set; }

        string Version { get; set; }

        string Description { get; set; }

        string ComponentAssembly { get; set; }

        string FullyQualifiedComponentName { get; set; }

        IEnumerable<string> RequiredProperties { get; set; }
    }
}
