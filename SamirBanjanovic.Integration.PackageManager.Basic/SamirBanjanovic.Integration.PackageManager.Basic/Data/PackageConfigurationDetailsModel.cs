using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.PackageManager.Core;

namespace OnTrac.Integration.PackageManager.Basic.Data
{
    public class PackageConfigurationDetailsModel
    {
        public Guid Id { get; set; }

        public string PackageName { get; set; }

        public DateTime InsertDateTime { get; set; }

        public int VersionIndex { get; set; }

        public string PackagePath { get; set; }

        public string PackageConfigurationFilePath { get; set; }

        public IPackageConfiguration PackageConfiguration { get; set; }
    }
}
