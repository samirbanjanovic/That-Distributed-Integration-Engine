using System;
using System.Collections.Generic;
using System.Text;
using TDIE.PackageManager.Core;

namespace TDIE.PackageManager.Basic.Data
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
