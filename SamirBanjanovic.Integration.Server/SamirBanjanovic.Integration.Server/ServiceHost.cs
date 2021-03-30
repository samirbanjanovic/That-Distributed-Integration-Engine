using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OnTrac.Integration.Server
{
    internal class Program
    {
        public static readonly IConfiguration Configuration = new ConfigurationBuilder()
              .SetBasePath(Process.GetCurrentProcess().MainModule.FileName)
              .AddJsonFile("appsettings.json", false, true)
              .Build();

        private static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                                     .ConfigureServices((hostContext, services) =>
                                     {
                                         services.AddLogging()
                                                 .AddOptions();
                                                 // will need expression builder to generate all this so we can 
                                                 // use the generic class
                                                 // or write our own class that is a hosted service and can add all the components                                                
                                     })
                                     .Build();
        }
    }
}
