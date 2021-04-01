using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDIE.Server.Core;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace TDIE.Server.Tests
{
    [TestClass]
    public class PackageLoaderTests
    {
        [TestMethod]
        public async Task TestPackageInsatll()
        {
            var installer = StaticResources.ServiceProvider.GetService<IComponentPackageInstaller>();

            var componentMetadata = await installer.TryInstallPackageAsync(new FileInfo(@"C:\drops\Integration\packages\test.cpkg"));

            Assert.AreNotEqual(null, componentMetadata);
        }
    }
}
