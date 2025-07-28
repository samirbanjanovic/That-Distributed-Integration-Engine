using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TDIE.PackageManager.Basic;

namespace BasicPackageManagerTests
{
    [TestClass]
    public class UnitTest1
    {
        public static readonly IConfiguration configuration = new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

        [TestMethod]
        public async Task TestConfigurationFileStructure()
        {
            var configJson = File.ReadAllText(@"C:\repos\Integration Platform\TDIE.Components.FileWatcher\TDIE.Components.FileWatcher\configuration.json");

            var config = JsonConvert.DeserializeObject<BasicPackageConfiguration>(configJson);
        }
    }
}
