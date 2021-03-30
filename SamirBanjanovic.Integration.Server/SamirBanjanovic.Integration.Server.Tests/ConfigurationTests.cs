using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTrac.Integration.Server.Classes;

namespace OnTrac.Integration.Server.Tests
{
    [TestClass]
    public class ConfigurationTests
    {

        [TestMethod]
        public void TestLoadingEngineConfiguration()
        {

            

            //var engineConfig = StaticResources.ServiceProvider.GetService<IOptions<PlatformConfiguration>>()?.Value;
            //var packageManagerSettings = StaticResources.ServiceProvider.GetService<IOptions<PackageInstallerSettings>>()?.Value;
            //var pacakgeExplorerSettings = StaticResources.ServiceProvider.GetService<IOptions<PackageExplorerSettings>>()?.Value;

            //Assert.IsNotNull(engineConfig);
            //Assert.IsNotNull(packageManagerSettings);
            //Assert.IsNotNull(pacakgeExplorerSettings);

            //Assert.IsNotNull(engineConfig.Name);
            //Assert.IsNotNull(engineConfig.Properties);
            //Assert.IsNotNull(engineConfig.Id);

            //Assert.IsNotNull(packageManagerSettings.InstallPath);
            //Assert.IsNotNull(packageManagerSettings.StagingPath);

            //Assert.IsNotNull(pacakgeExplorerSettings.PackageExtension);
            //Assert.IsNotNull(pacakgeExplorerSettings.PackagesDropDirectory);                        
        }
    }
}
