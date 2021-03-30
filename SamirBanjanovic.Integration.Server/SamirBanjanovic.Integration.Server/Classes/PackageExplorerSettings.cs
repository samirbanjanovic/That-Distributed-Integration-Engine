using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Classes
{
    public sealed class PackageExplorerSettings
    {
        public string PackagesDropDirectory { get; set; }

        public bool ContinouslyMonitor { get; set; }

        public string PackageExtension { get; set; }
    }
}
