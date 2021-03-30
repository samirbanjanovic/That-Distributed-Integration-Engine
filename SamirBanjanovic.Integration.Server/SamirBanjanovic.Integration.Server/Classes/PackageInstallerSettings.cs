
using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Classes
{
    public sealed class PackageInstallerSettings
    {
        public string StagingPath { get; set; }

        public string DestinationPath { get; set; }

        public string InformationFile { get; set; }
    }
}
