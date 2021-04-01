using Microsoft.Extensions.Configuration;
using TDIE.Components.Master;
using TDIE.Components.Master.WebApi;
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
                    {"masterConfigurationFile" , "masterComponentWebConfig.json" }
                }
            };

            var serverConfigFile = settings.Properties["masterConfigurationFile"];

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(serverConfigFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            await new MasterComponent(settings, null)
                .StartAsync(CancellationToken.None);

            Console.ReadLine();         
        }
    }
}
