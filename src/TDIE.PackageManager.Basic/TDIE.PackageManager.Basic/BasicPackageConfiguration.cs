﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TDIE.PackageManager.Core;

namespace TDIE.PackageManager.Basic
{
    public class BasicPackageConfiguration
        : IPackageConfiguration
    {
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public string Description { get; set; }
        public string ContentRoot { get; set; }
        public IDictionary<string, string> ExtensionProperties { get; set; }
    }
}
