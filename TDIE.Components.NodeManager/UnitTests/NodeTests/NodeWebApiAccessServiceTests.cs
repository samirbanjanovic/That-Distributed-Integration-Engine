using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TDIE.Components.NodeManager.Classes;
using TDIE.Components.NodeManager.Node.AccessService;
using TDIE.Components.NodeManager.Node.Data.Entities;
using TDIE.Components.NodeManager.Node.WebApi;

namespace TDIE.Components.NodeManager.Tester.NodeTests
{
    [TestClass]
    public class NodeWebApiAccessServiceTests
    {
        private static readonly NodeServer _localNode = new NodeServer
        {
            NetworkName = "LT10FL9SBH2",
            NetworkIp = "10.198.30.27",
            NodeApiUri = "https://localhost:5100"
        };
        private static readonly INodeAccessService _nodeWebApiAccessService = new NodeWebApiAccessService(_localNode);


        #region package management tests

        [TestMethod]
        public async Task TestUploadPackageAsync()
        {
            PackageDetails packageDetails = null;
            using (var file = File.OpenRead(@"C:\repos\Integration Platform\TDIE.Components.NodeManager\UnitTests\p4.zip"))
            {
                packageDetails = await _nodeWebApiAccessService.UploadPackageAsync(file);
            }

            Assert.IsNotNull(packageDetails);
            Assert.AreEqual("componentHostProcess", packageDetails.PackageName);
            Assert.AreEqual("1.0.0", packageDetails.PackageVersion);
            Assert.IsTrue(packageDetails.ExtensionProperties.ContainsKey("command"));
            Assert.IsTrue(packageDetails.ExtensionProperties.ContainsKey("args"));
        }

        [TestMethod]
        public async Task TestUpdatePackageAsync()
        {

            // submit data to api and read data from api
            PackageDetails submittedPackageDetails = null;
            PackageDetails retrievedPackageDetails = null;

            using (var file = File.OpenRead(@"C:\repos\Integration Platform\TDIE.Components.NodeManager\UnitTests\p4_config.zip"))
            {
                submittedPackageDetails = await _nodeWebApiAccessService.UpdatePackageAsync(file);
                retrievedPackageDetails = await _nodeWebApiAccessService.GetNodePackageConfigurationAsync("componentHostProcess");
            }

            // read local file for check
            PackageDetails localConfig = null;
            var jsonSerializer = new JsonSerializer();
            using (var file = File.OpenRead(@"C:\repos\Integration Platform\TDIE.Components.NodeManager\UnitTests\configuration.json"))
            {
                using (TextReader reader = new StreamReader(file))
                {
                    localConfig = (PackageDetails)jsonSerializer.Deserialize(reader, typeof(PackageDetails));
                }
            }


            Assert.IsNotNull(submittedPackageDetails);
            Assert.AreEqual(localConfig.Description, submittedPackageDetails.Description);
            Assert.AreEqual(localConfig.PackageVersion, submittedPackageDetails.PackageVersion);
            Assert.IsTrue(submittedPackageDetails.ExtensionProperties.ContainsKey("command"));
            Assert.IsTrue(submittedPackageDetails.ExtensionProperties.ContainsKey("args"));


            Assert.IsNotNull(submittedPackageDetails);
            Assert.AreEqual(localConfig.Description, retrievedPackageDetails.Description);
            Assert.AreEqual(localConfig.PackageVersion, retrievedPackageDetails.PackageVersion);
            Assert.IsTrue(retrievedPackageDetails.ExtensionProperties.ContainsKey("command"));
            Assert.IsTrue(retrievedPackageDetails.ExtensionProperties.ContainsKey("args"));
        }

        #endregion package management tests

        #region process management and stats

        private Guid _remoteNodeProcessId;

        [TestMethod]
        public async Task TestStartProcessAsync()
        {
            string packageName = "componentHostProcess";
            var args = new Dictionary<string, string>
            {
                { "--port", "10000" }
            };

            var processInfo = await _nodeWebApiAccessService.StartProcessAsync(packageName, (1, args));
            _remoteNodeProcessId = processInfo.NodeProcessId;

            Assert.IsNotNull(processInfo);
            Assert.IsNotNull(processInfo.NodeProcessId);
            Assert.IsTrue(!string.IsNullOrEmpty(processInfo.ProcessUri));
            Assert.AreEqual("componentHostProcess", processInfo.PackageName);
            Assert.AreEqual(args["--port"], processInfo.Arguments["--port"]);

        }

        [TestMethod]
        public async Task TestGetNodeStatsAsync()
        {
            var stats = await _nodeWebApiAccessService.GetNodeStatsAsync();

            Assert.IsNotNull(stats, "failed to retrieve stats");
            Assert.AreEqual(Environment.MachineName, stats.Environment.MachineName);
            Assert.IsNotNull(stats.PackageInstanceDetails);
            Assert.IsTrue(stats.PackageInstanceDetails.Count() > 0);

            Assert.IsNotNull(stats.PackageInstanceDetails.FirstOrDefault().Command);
            Assert.IsNotNull(stats.PackageInstanceDetails.FirstOrDefault().PackageName);
            Assert.IsTrue(stats.PackageInstanceDetails.FirstOrDefault().Count > 0);
        }

        [TestMethod]
        public async Task TestGetNodeProcessStatsAsync()
        {
            var processes = await _nodeWebApiAccessService.GetNodeProcessStatsAsync();

            Assert.IsNotNull(processes);
            Assert.IsTrue(processes.Count() > 0);
            Assert.AreEqual("componentHostProcess", processes.FirstOrDefault().PackageName);
            Assert.IsNotNull(processes.Select(x => x.Arguments).ToList());
            Assert.IsTrue(processes.Select(x => x.Arguments).ToList().Count > 0);
        }

        [TestMethod]
        public async Task TestGetNodeProcessStatsForPackageAsync()
        {
            string packageName = "componentHostProcess";
            var processes = await _nodeWebApiAccessService.GetNodeProcessStatsAsync(packageName);

            Assert.IsNotNull(processes, $"No stats for given package {packageName}");
            Assert.IsTrue(processes.Count() > 0);
        }


        [TestMethod]
        public async Task TestKillProcessAsync()
        {
            string packageName = "componentHostProcess";
            var nodeProcesses = await _nodeWebApiAccessService.GetNodeProcessStatsAsync(packageName);

            foreach (var process in nodeProcesses)
            {

                var stopResponse = await _nodeWebApiAccessService.KillProcessAsync(process.PackageName, process.NodeProcessId);

                Assert.AreEqual(packageName, process.PackageName);
                Assert.IsTrue(stopResponse);
            }
        }

        [TestMethod]
        public async Task TestKillProcessByPackageNameAsync()
        {
            await TestStartProcessAsync();

            string packageName = "componentHostProcess";
            var nodeProcesses = await _nodeWebApiAccessService.GetNodeProcessStatsAsync(packageName);

            foreach (var process in nodeProcesses)
            {

                var stopResponse = await _nodeWebApiAccessService.KillProcessAsync(process.PackageName, process.NodeProcessId);

                Assert.AreEqual(packageName, process.PackageName);
                Assert.IsTrue(stopResponse);
            }
        }

        [TestMethod]
        public async Task TestKillProcessByPackageNameInstanceAndGuidAsync()
        {
            await TestStartProcessAsync();

            string packageName = "componentHostProcess";
            var nodeProcesses = await _nodeWebApiAccessService.GetNodeProcessStatsAsync(packageName);

            var stopResponse = await _nodeWebApiAccessService.KillProcessesAsync();

            Assert.IsTrue(stopResponse);
        }

        [TestMethod]
        public async Task TestKillProcessByPackageNameInstanceAsync()
        {
            await TestStartProcessAsync();

            string packageName = "componentHostProcess";
            var nodeProcesses = await _nodeWebApiAccessService.GetNodeProcessStatsAsync(packageName);

            var stopResponse = await _nodeWebApiAccessService.KillProcessesAsync();

            Assert.IsTrue(stopResponse);
        }

        #endregion process management tests
    }
}
