using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDIE.Components.NodeManager.ComponentHost.Data.Entities;
using TDIE.Components.NodeManager.ComponentHost.WebApi;

namespace UnitTests.ComponentHostTests
{
    [TestClass]
    public class ComponentHostWebApiAccessServiceTests
    {
        private static readonly ComponentHostInstanceSettings _componentInstanceSettings = new ComponentHostInstanceSettings
        {
            Id = 1,
            PackageName = "fileWatcherComponent",
            PackageVersion = "1.0.0",
            Settings = new Dictionary<string, string>
            {
                {"type", "watcher" },
                {"path", "c:\\" },
                {"filter", "*.csv" }
            }
        };
        

        //static ComponentHostWebApiAccessServiceTests()
        //{
        //    _componentInstanceSettings.SetDynamicApiUri("https://localhost:5110");
        //}
        
        #region host management tests

        [TestMethod]
        public async Task TestGetComponentHostInformationAsync()
        {
            //var results = await _componentHostWebApiAccessService.GetComponentHostInformationAsync(_componentHostUri);

            //Assert.IsNotNull(results);
            //Assert.AreEqual("AwaitingConfiguration", results.HostState);
            //Assert.IsNull(results.MessagePublisher);
            //Assert.IsNull(results.Component);
        }

        #endregion host management tests

        #region package management tests
        #endregion package management tests

        #region hosted service tests
        #endregion hosted service tests
    }
}
