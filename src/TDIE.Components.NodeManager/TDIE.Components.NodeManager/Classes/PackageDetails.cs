﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.NodeManager.Classes
{
    public class PackageDetails
    {
        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public string Description { get; set; }

        public string ContentRoot { get; set; }

        public IDictionary<string, string> ExtensionProperties { get; set; }
    }
}
