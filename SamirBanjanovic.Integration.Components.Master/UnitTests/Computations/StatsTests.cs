using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.Computations;

namespace UnitTests.Computations
{

    [TestClass]
    public class StatsTests
    {
        [TestMethod]
        public void TestComputeComponentEveness()
        {
            //formula used from: https://sciencing.com/calculate-species-evenness-2851.html
            //10 orchids
            //40 roses
            //100 marigolds

            var eveness =
            Enumerable.Repeat(new ComponentHostInstanceSettings
            {
                PackageName = "orchids",
                PackageVersion = "1.0.0",
                Id = 0
            }, 60)
            .Concat(Enumerable.Repeat(new ComponentHostInstanceSettings
            {
                PackageName = "roses",
                PackageVersion = "1.0.0",
                Id = 0
            }, 10))
            .Concat(Enumerable.Repeat(new ComponentHostInstanceSettings
            {
                PackageName = "marigolds",
                PackageVersion = "1.0.0",
                Id = 0
            }, 25))
            .Concat(Enumerable.Repeat(new ComponentHostInstanceSettings
            {
                PackageName = "dandalions",
                PackageVersion = "1.0.0",
                Id = 0
            }, 1))
            .Concat(Enumerable.Repeat(new ComponentHostInstanceSettings
            {
                PackageName = "figs",
                PackageVersion = "1.0.0",
                Id = 0
            }, 4))
            .Evenness();

        }
    }
}
