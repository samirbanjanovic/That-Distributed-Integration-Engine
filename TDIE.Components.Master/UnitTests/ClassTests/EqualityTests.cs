using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TDIE.Components.Master.Cluster.Classes;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Node.AccessService.Classes;

namespace UnitTests.ClassTests
{
    [TestClass]
    public class EqualityTests
    {
        [TestMethod]
        public void TestComponentInstanceEquality()
        {
            var obj1 = new ComponentHostInstanceSettings()
            {
                Id = 1,
                PackageName = "TestPackage",
                PackageVersion = "1.0.0",
                Settings = null
            };

            var obj2 = new ComponentHostInstanceSettings()
            {
                Id = 1,
                PackageName = "TestPackage",
                PackageVersion = "1.0.0",
                Settings = null
            };

            var obj3 = new ComponentHostInstanceSettings()
            {
                Id = 2,
                PackageName = "TestPackage2",
                PackageVersion = "1.0.0",
                Settings = null
            };

            var obj1EqualsObj2 = obj1.Equals(obj2);

            Assert.IsTrue(obj1EqualsObj2);
        }

        [TestMethod]
        public void TestNodeComponentEqualityComparer()
        {
            var nodeProcessId = Guid.NewGuid();
            var startDateTime = DateTime.Now;

            var nodeProcessId2 = Guid.NewGuid();
            var startDateTime2 = DateTime.Now;

            var nc1 = new NodeComponent
            {
                //Component = new ComponentInstanceSettings()
                //{
                //    Id = 1,
                //    PackageName = "TestPackage",
                //    PackageVersion = "1.0.0",
                //    Settings = null
                //},
                //ProcessInformation = new NodeBasicProcessInformation
                //{
                //    Command = "some command",
                //    MachineName = "machine1",
                //    NodeProcessId = nodeProcessId,
                //    PackageName = "some package",
                //    ProcessUri = "someURI",
                //    StartDateTime = startDateTime,
                //    SystemProcessId = 1234
                //}
            };
            var nc2 = new NodeComponent
            {
                //Component = new ComponentInstanceSettings()
                //{
                //    Id = 2,
                //    PackageName = "TestPackage2",
                //    PackageVersion = "1.0.0",
                //    Settings = null
                //},
                //ProcessInformation = new NodeBasicProcessInformation
                //{
                //    Command = "some command2",
                //    MachineName = "machine1",
                //    NodeProcessId = nodeProcessId2,
                //    PackageName = "some package",
                //    ProcessUri = "someURI",
                //    StartDateTime = startDateTime2,
                //    SystemProcessId = 1235
                //}
            };
            var nc3 = new NodeComponent
            {
                //Component = new ComponentInstanceSettings()
                //{
                //    Id = 1,
                //    PackageName = "TestPackage",
                //    PackageVersion = "1.0.0",
                //    Settings = null
                //},
                //ProcessInformation = new NodeBasicProcessInformation
                //{
                //    Command = "some command",
                //    MachineName = "machine1",
                //    NodeProcessId = nodeProcessId,
                //    PackageName = "some package",
                //    ProcessUri = "someURI",
                //    StartDateTime = startDateTime,
                //    SystemProcessId = 1234
                //}
            };

            //var hashSet = new HashSet<NodeComponent>(new[] { nc1, nc2 }, new NodeComponentEqualityComparer());
            
            // check if hashset contains object with same entries;
            //var contains = hashSet.Contains(nc3);

            //Assert.IsTrue(contains);
        }


    }
}
