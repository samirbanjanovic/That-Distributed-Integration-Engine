using Microsoft.Extensions.Configuration;
using TDIE.Components.NodeManager;
using TDIE.Components.NodeManager.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ComponentLaunchTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = new ComponentSettings
            {
                Id = -1,
                Name = "selfHosted",
                Properties = new Dictionary<string, string>
                {
                    {"masterConfigurationFile" , "NodeManagerComponentWebConfig.json" }
                }
            };

            var serverConfigFile = settings.Properties["masterConfigurationFile"];

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(serverConfigFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            await new NodeManagerComponent(settings, null)
                .StartAsync(CancellationToken.None);

            Console.ReadLine();         
        }
    }
}
