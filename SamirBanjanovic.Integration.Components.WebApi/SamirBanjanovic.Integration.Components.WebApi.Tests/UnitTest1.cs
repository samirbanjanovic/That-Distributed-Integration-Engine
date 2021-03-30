using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OnTrac.Integration.Components.WebApi.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ComponentInitTest()
        {
            var settings = new ComponentSettings
            {
                Id = 1,
                Name = "WebApi",
                Properties = new Dictionary<string, string>()
                {
                    {"url", "http://localhost:9337" },
                    {"type" , "webApi" }
                }
            };

            var component = new WebApiEndpointComponent(settings, null, null);
            component.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
