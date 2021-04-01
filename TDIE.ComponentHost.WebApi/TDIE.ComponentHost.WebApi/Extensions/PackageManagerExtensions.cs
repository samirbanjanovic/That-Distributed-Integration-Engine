using System.IO;
using TDIE.PackageManager.Core;

namespace TDIE.ComponentHost.WebApi.Extensions
{
    public static class PackageManagerExtensions
    {
        public static string GetQualifiedAssmeblyPath(this IPackageConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.ExtensionProperties["AssemblyExtension"]) || configuration.ExtensionProperties["AssemblyName"].EndsWith(configuration.ExtensionProperties["AssemblyExtension"]))
            {
                return configuration.ExtensionProperties["AssemblyName"];
            }

            return Path.ChangeExtension(configuration.ExtensionProperties["AssemblyName"], configuration.ExtensionProperties["AssemblyExtension"]);
        }
    }
}
