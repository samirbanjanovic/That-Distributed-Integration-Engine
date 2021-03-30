using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.PackageManager.Core
{
    public interface IPackageConfiguration
    {
        string PackageName { get; set; }
        string PackageVersion { get; set; }
        string Description { get; set; }
        string ContentRoot { get; set; }     
        IDictionary<string, string> ExtensionProperties { get; set; }
    }
}
