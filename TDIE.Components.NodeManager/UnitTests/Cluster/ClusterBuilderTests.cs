using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTableExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TDIE.Components.NodeManager.ComponentHost.Data;
using TDIE.Components.NodeManager.Node.Data;

namespace UnitTests.Cluster
{
    [TestClass]
    public class ClusterBuilderTests
    {
        class Group
        {
            public string Package { get; set; }
            public int Size { get; set; }

            public override string ToString()
            {
                return $"{Package} - {Size}";
            }
        }

        class Cluster
        {
            public string Node { get; set; }
            public IEnumerable<Group> Groups { get; set; }
            public double Evenness { get; set; }

            public override string ToString()
            {
                return $"{Node}\t\t|\t{string.Join(" : ", Groups?.Select(x => x.ToString()))}\t\t|\t\t{Evenness}";
            }
        }


        IComponentHostSettingsDataAccess _componentSettingsDataAccess = new JsonComponentHostSettingsAccessService();
        INodeSettingsDataAccess _nodeSettingsDataAccess = new JsonNodeSettingsAccessService();

        //[TestMethod]
        //public async Task TestClusterAssignment()
        //{
        

        //    var services = await _componentSettingsDataAccess.GetComponentInstanceSettingsAsync();
        //    var nodes = await _nodeSettingsDataAccess.GetNodesAsync();

        //    var assignments = nodes.PairComponentInstances(services);


        //    // output results
        //    var verification = assignments.Select(x => new Cluster
        //    {
        //        Node = x.Node.NetworkName,
        //        Groups = x.ComponentInstances.GroupBy(g => g.PackageName)
        //                           .Select(c => new Group()
        //                           {
        //                               Package = c.Key,
        //                               Size = c.Count()
        //                           })
        //                           .ToList(),
        //       Evenness = x.Evenness
        //    }).ToList();

        //    Debug.Write(string.Join("\r\n", verification.Select(x => x.ToString())));
        //    Assert.IsTrue(assignments.Min(x => x.Evenness > 0.75));
        //}
    }
}
