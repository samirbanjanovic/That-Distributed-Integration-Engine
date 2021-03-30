using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace OnTrac.Integration.PackageManager.Basic.Data
{
    internal static class LiteDbCollectionExtensions
    {
        public static LiteCollection<PackageConfigurationDetailsModel> GetPackageConfigurationDetailsModelCollection(this LiteDatabase liteDatabase)
        {
            var configurationModels = liteDatabase.GetCollection<PackageConfigurationDetailsModel>();
            configurationModels.EnsureIndex(x => x.Id, true);
            configurationModels.EnsureIndex(x => x.PackageName, false);            
            return configurationModels;
        }
    }
}
