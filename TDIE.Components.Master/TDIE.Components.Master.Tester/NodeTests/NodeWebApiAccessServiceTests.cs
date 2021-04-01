using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDIE.Components.Master.Node.Data.Entities;
using TDIE.Components.Master.Node.WebApi;
using TDIE.Components.Master.Node.WebApi.Classes;

namespace TDIE.Components.Master.Tester.NodeTests
{
    [TestClass]
    public class NodeWebApiAccessServiceTests
    {
        private static readonly INodeWebApiAccessService _nodeWebApiAccessService = new NodeWebApiAccessService(null, null);
        private static readonly NodeServer _testNode = new NodeServer
        {
            NetworkName = "LT10FL9SBH2",
            NetworkIp = "10.198.30.27",
            NodeApiUrl = "https://LT10FL9SBH2:5000/api/node"
        };

        [TestMethod]
        public async Task TestGetProcessStats()
        {
            var stats = await _nodeWebApiAccessService.GetNodeStats(_testNode);

        }
    }
}
