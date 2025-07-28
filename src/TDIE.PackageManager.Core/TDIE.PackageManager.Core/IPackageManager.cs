using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TDIE.PackageManager.Core
{
    public interface IPackageManager
    {
        string PackageRoot { get; set; }

        Task<IEnumerable<IPackageConfiguration>> GetAllPackageConfigurationsAsync();

        Task<IPackageConfiguration> ImportPackageAsync(Stream stream);

        Task<IPackageConfiguration> UpdatePackageAsync(Stream stream);

        Task<IPackageConfiguration> DeletePackageAsync(string packageName);
                
        Task<IPackageConfiguration> GetPackageConfigurationAsync(string packageName);

        Task<string> GetPackagePathAsync(string packageName);

        Task<string> GetPackageContentRootAsync(string packageName);

        
    }
}
