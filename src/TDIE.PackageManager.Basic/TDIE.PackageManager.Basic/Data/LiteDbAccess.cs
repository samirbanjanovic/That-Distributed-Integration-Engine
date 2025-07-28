using System;
using LiteDB;
using TDIE.PackageManager.Core;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace TDIE.PackageManager.Basic.Data
{
    internal class LiteDbAccess
        : IDisposable
    {        
        private readonly LiteCollection<PackageConfigurationDetailsModel> _configurationModels;
        private readonly LiteDatabase _database;

        private readonly string _dbPath;
        public LiteDbAccess(string dbPath)
        {
            _dbPath = dbPath;
            CreateDbPath();
            _database = new LiteDatabase(_dbPath);
            _configurationModels = _database.GetPackageConfigurationDetailsModelCollection();            
        }

        public void InsertPackageDetails(string packagePath, string relativeConfig, IPackageConfiguration configuration) => _configurationModels.Insert(BuildPackageConfigurationDetailsModel(packagePath, relativeConfig, configuration));

        public IEnumerable<PackageConfigurationDetailsModel> GetAllPackageConfigurations() => _configurationModels.FindAll().ToList();

        public PackageConfigurationDetailsModel GetPackageConfigurationDetails(string packageName)
        {
            var latest = _configurationModels.Find(x => x.PackageName == packageName).Max(x => x.VersionIndex);

            return _configurationModels  
                        .FindOne(x => x.PackageName == packageName &&
                                      x.VersionIndex == latest);

        }

        public void DeletePackageDetails(string packageName) => _configurationModels.Delete(x => x.PackageName == packageName);

        private PackageConfigurationDetailsModel BuildPackageConfigurationDetailsModel(string packagePath, string relativeConfigPath, IPackageConfiguration configuration) =>
            new PackageConfigurationDetailsModel
            {
                Id = Guid.NewGuid(),
                PackagePath = packagePath,
                InsertDateTime = DateTime.Now,
                PackageName = configuration.PackageName,
                VersionIndex = GetPackageConfiugrationVersion(configuration),
                PackageConfigurationFilePath = relativeConfigPath,
                PackageConfiguration = configuration,
            };
        

        private int GetPackageConfiugrationVersion(IPackageConfiguration configuration)
        {
            int versionIndex = 0;
            var configs = _configurationModels.Find(p => p.PackageName == configuration.PackageName);
            if (configs.Any())
            {
                versionIndex = configs.Max(y => y.VersionIndex) + 1;
            }

            return versionIndex;
        }

        public void Dispose() => _database.Dispose();

        private void CreateDbPath()
        {
            var dirPath = Path.GetDirectoryName(Path.GetFullPath(_dbPath));
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

    }
}
