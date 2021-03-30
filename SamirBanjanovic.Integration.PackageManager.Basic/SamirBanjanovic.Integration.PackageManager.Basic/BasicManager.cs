using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnTrac.Integration.PackageManager.Basic.Data;
using OnTrac.Integration.PackageManager.Core;
using SysDirectory = System.IO.Directory;

namespace OnTrac.Integration.PackageManager.Basic
{
    public class BasicManager
        : IPackageManager
    {
        public BasicManager(IConfiguration configuration, ILogger<BasicManager> logger)
        {
            PackageRoot = Path.GetFullPath(configuration["Configuration:PackageManager:Directory"]);
            ConfigurationFileName = configuration["Configuration:PackageManager:ConfigurationFileName"];
            StorePath = configuration["Configuration:PackageManager:StorePath"] as string;
        }

        public string PackageRoot { get; set; }
        public string ConfigurationFileName { get; set; }
        public string StorePath { get; }


        public Task<IEnumerable<IPackageConfiguration>> GetAllPackageConfigurationsAsync()
        {
            using (var storeAcces = new LiteDbAccess(StorePath))
            {
                return Task.FromResult(storeAcces.GetAllPackageConfigurations().Select(x => x.PackageConfiguration));
            }
        }

        public async Task<IPackageConfiguration> GetPackageConfigurationAsync(string packageName)
        {
            IPackageConfiguration configuration = null;
            var path = await GetPackagePathAsync(packageName);

            var configurations = SysDirectory.GetFiles(path, ConfigurationFileName, SearchOption.AllDirectories);

            if (configurations.Any())
            {
                var configurationPath = configurations.FirstOrDefault();

                configuration = JsonConvert.DeserializeObject<BasicPackageConfiguration>(File.ReadAllText(configurationPath));
            }

            return configuration;
        }

        public Task<string> GetPackagePathAsync(string packageName)
        {
            using (var storeAcces = new LiteDbAccess(StorePath))
            {
                return Task.FromResult(storeAcces.GetPackageConfigurationDetails(packageName).PackagePath);
            }
        }


        public Task<string> GetPackageContentRootAsync(string packageName)
        {
            using (var storeAcces = new LiteDbAccess(StorePath))
            {
                return Task.FromResult(Path.Combine(PackageRoot, storeAcces.GetPackageConfigurationDetails(packageName).PackageConfiguration.ContentRoot));
            }
        }

        public Task<IPackageConfiguration> ImportPackageAsync(Stream stream)
        {
            return Task.FromResult(ImportOrUpdatePackage(stream, false));
        }


        public Task<IPackageConfiguration> UpdatePackageAsync(Stream stream)
        {
            return Task.FromResult(ImportOrUpdatePackage(stream, true));
        }

        private IPackageConfiguration ImportOrUpdatePackage(Stream stream, bool isUpdate)
        {
            using (var zipArchive = new ZipArchive(stream))
            {
                string relativeConfigFilePath = GetRelativeConfigFilePath(zipArchive);

                using (var configFileStream = zipArchive.GetEntry(relativeConfigFilePath).Open())
                {
                    using (var streamReader = new StreamReader(configFileStream))
                    {
                        IPackageConfiguration configuration = JsonConvert.DeserializeObject<BasicPackageConfiguration>(streamReader.ReadToEnd());

                        if (configuration is null)
                        {
                            throw new InvalidDataException($"Incomplete content structure. Missing configuration file of name {ConfigurationFileName}");
                        }

                        var packagePath = Path.Combine(PackageRoot, configuration.PackageName);

                        zipArchive.ExtractToDirectory(packagePath, isUpdate);
                        using (var storeAcces = new LiteDbAccess(StorePath))
                        {
                            storeAcces.InsertPackageDetails(packagePath, relativeConfigFilePath, configuration);
                        }

                        return configuration;
                    }
                }
            }
        }

        public Task<IPackageConfiguration> DeletePackageAsync(string packageName)
        {
            using (var storeAccess = new LiteDbAccess(StorePath))
            {
                var packageDetails = storeAccess.GetPackageConfigurationDetails(packageName);

                if (packageDetails is null)
                {
                    return null;
                }

                var packageDir = Path.Combine(PackageRoot, packageName);

                if (SysDirectory.Exists(packageDir))
                {
                    SysDirectory.Delete(packageDir, true);
                }

                storeAccess.DeletePackageDetails(packageName);

                return Task.FromResult(packageDetails.PackageConfiguration);
            }
        }


        private string GetRelativeConfigFilePath(ZipArchive zipArchive)
        {
            var configEntries = zipArchive.Entries.Where(x => x.Name == ConfigurationFileName);

            if (!configEntries.Any(x => x.Name == ConfigurationFileName))
            {
                throw new FileNotFoundException(ConfigurationFileName);
            }

            return zipArchive.Entries.FirstOrDefault(x => x.Name.Contains(ConfigurationFileName)).FullName;
        }

        private void ArchivePackageIfExists(string packagePath)
        {
            if (SysDirectory.Exists(packagePath))
            {
                SysDirectory.Move(packagePath, $"{packagePath}.{DateTime.Now.ToString("yyyyMMddHHmmssff")}.{Guid.NewGuid().ToString("N")}");
            }
        }
    }
}
