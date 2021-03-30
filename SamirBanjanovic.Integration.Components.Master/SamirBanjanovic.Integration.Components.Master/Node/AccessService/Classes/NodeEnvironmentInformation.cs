using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Components.Master.Node.AccessService.Classes
{
    public class NodeEnvironmentInformation
    {
        public string MachineName { get; set; }

        public string OsPlatform { get; set; }

        public OperatingSystem OSVersion { get; set; }

        public bool Is64BitOperatingSystem { get; set; }

        public string Framework { get; set; }

        public string FrameworkDescription { get; set; }

        public bool UserInteractive { get; set; }

        public string CurrentDirectory { get; set; }

        public int ProcessorCount { get; set; }        
    }
}
