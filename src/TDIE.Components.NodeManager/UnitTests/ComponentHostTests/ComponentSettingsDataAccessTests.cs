using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDIE.Components.NodeManager.ComponentHost.Data;

namespace UnitTests.ComponentHostTests
{
    [TestClass]
    public class ComponentSettingsDataAccessTests
    {
        IComponentHostSettingsDataAccess _componentSettingsDataAccess = new JsonComponentHostSettingsAccessService();

        [TestMethod]
        public async Task TestGetComponentInstanceSettings()
        {
            var componentSettings = (await _componentSettingsDataAccess.GetComponentInstanceSettingsAsync()).ToArray();

            Assert.IsNotNull(componentSettings);
            Assert.IsTrue(componentSettings.Count() > 0);
            Assert.AreEqual(1, componentSettings[0].Id);
            Assert.AreEqual("fileWatcherComponent", componentSettings[0].PackageName);

            Assert.AreEqual("watcher", componentSettings[0].Settings["type"]);
            Assert.AreEqual("C:\\drops\\Test\\boxbot\\inbound", componentSettings[0].Settings["path"]);
            Assert.AreEqual("*.csv", componentSettings[0].Settings["filter"]);

            Assert.AreEqual("watcher", componentSettings[1].Settings["type"]);
            Assert.AreEqual("C:\\drops\\Test\\IT Test", componentSettings[1].Settings["path"]);
            Assert.AreEqual("*.txt", componentSettings[1].Settings["filter"]);
        }

        [TestMethod]
        public async Task TestGetComponentPackages()
        {
            var componentSettings = (await _componentSettingsDataAccess.GetComponentPackagesAsync()).ToArray();

            Assert.IsNotNull(componentSettings);
            Assert.IsTrue(componentSettings.Count() > 0);
            Assert.AreEqual("fileWatcherPackage", componentSettings[0].Name);
            Assert.AreEqual("C:\\repos\\Integration Platform\\TDIE.ComponentHost.WebApi\\TDIE.ComponentHost.WebApi\\bin\\Release\\netcoreapp2.2\\p2.zip", componentSettings[0].PackagePath);

            Assert.AreEqual("quartzSchedulerPackage", componentSettings[1].Name);
            Assert.AreEqual("C:\\repos\\Integration Platform\\TDIE.ComponentHost.WebApi\\TDIE.ComponentHost.WebApi\\bin\\Release\\netcoreapp2.2\\p3.zip", componentSettings[1].PackagePath);

            Assert.AreEqual("basicMessagePublisher", componentSettings[2].Name);
            Assert.AreEqual("C:\\repos\\Integration Platform\\TDIE.ComponentHost.WebApi\\TDIE.ComponentHost.WebApi\\bin\\Release\\netcoreapp2.2\\p4.zip", componentSettings[2].PackagePath);
        }
    }
}
