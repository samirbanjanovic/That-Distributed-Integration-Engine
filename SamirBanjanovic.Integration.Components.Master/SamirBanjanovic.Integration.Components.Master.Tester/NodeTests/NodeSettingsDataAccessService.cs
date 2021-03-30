using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTrac.Integration.Components.Master.Node.Data;


namespace OnTrac.Integration.Components.Master.Tester.NodeTests
{
    [TestClass]
    public class NodeSettingsDataAccessService
    {
        INodeSettingsDataAccess _nodeSettingsDataAccess = new JsonNodeSettingsAccessService();

        [TestMethod]
        public async Task TestGetNodesAsync()
        {
            var nodes = (await _nodeSettingsDataAccess.GetNodesAsync()).ToArray();

            Assert.IsNotNull(nodes, "failed to get nodes from data store");
            Assert.IsTrue(nodes.Length == 3);
            Assert.AreEqual("LOUTFS01", nodes[0].NetworkName);
            Assert.AreEqual("10.198.10.21", nodes[0].NetworkIp);
            Assert.AreEqual("https://LOUTFS01:5000/api/node", nodes[0].NodeApiUrl);
        }

        [TestMethod]
        public async Task TestGetNodePackageAsync()
        {
            var nodePackages = (await _nodeSettingsDataAccess.GetNodePackagesAsync()).ToArray();

            Assert.IsNotNull(nodePackages, "failed to get nodes from data store");
            Assert.IsTrue(nodePackages.Length == 1);
            Assert.AreEqual("ComponentHostProcess", nodePackages[0].Name);
            Assert.AreEqual("1.0.0", nodePackages[0].Version);
            Assert.AreEqual("C:\\repos\\Integration Platform\\OnTrac.Integration.ComponentHost.WebApi\\OnTrac.Integration.ComponentHost.WebApi\\bin\\Release\\netcoreapp2.2\\p2.zip"
                , nodePackages[0].PackagePath);
        }
    }
}
