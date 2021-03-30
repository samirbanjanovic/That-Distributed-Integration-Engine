using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnTrac.Integration.Components.Master.WebApi
{
    public class Program
    {
        private readonly IConfiguration _configuration;
        

        public Program(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddJsonFile("masterComponentWebConfig.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            await new Program(configuration).RunServerAsync(CancellationToken.None);
        }
        
        internal async Task StartServerAsync(CancellationToken cancellationToken)
            => await CreateWebHostBuilder(_configuration).Build().StartAsync(cancellationToken);


        internal async Task RunServerAsync(CancellationToken cancellationToken)
            => await CreateWebHostBuilder(_configuration).Build().RunAsync(cancellationToken);
        
        
        private static IWebHostBuilder CreateWebHostBuilder(IConfiguration configuration, params string[] args) =>
                                WebHost.CreateDefaultBuilder(args ?? Array.Empty<string>())
                                        .UseKestrel()
                                        .UseSerilog()
                                        .UseUrls($"https://localhost:7777")
                                       //.ConfigureKestrel((context, options) =>
                                       //{
                                       //    options.Configure(configuration.GetSection("Kestrel"));
                                       //})
                                       .UseConfiguration(configuration)
                                       .UseContentRoot(Environment.CurrentDirectory)
                                       .UseStartup<Startup>();
    }
}
