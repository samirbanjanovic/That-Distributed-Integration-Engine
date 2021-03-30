using System.IO;
using Newtonsoft.Json;
using OnTrac.Integration.PackageManager.Core;
using System.Linq;

namespace OnTrac.Integration.NodeApi.Extensions
{
    public static class PackageConfigurationExtensions
    {
        public static string GetCommandPath(this IPackageConfiguration packageConfiguration, string packagesRoot)
        {
            return Path.Combine(packagesRoot,packageConfiguration.PackageName, packageConfiguration.ContentRoot, packageConfiguration.ExtensionProperties["command"]);
        }

    }
}
