using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using TDIE.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;

namespace TDIE.NodeApi
{
    public class Program
    {
        private static readonly IConfiguration _configuration;

        static Program()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            _configuration = new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)                        
                        .AddEnvironmentVariables()
                        .Build();
        }

        public static async Task<int> Main(string[] args)
        {// configure serilog
            Log.Logger = new LoggerConfiguration()
                            .Configure(_configuration)
                            .CreateLogger();

            try
            {
                Log.Logger.Information("{Message}", "Starting webhost");
                var webHost = CreateWebHostBuilder(args).Build();

                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{Message}", "Access API terminated unexpectedly");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
            return 0;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .ConfigureKestrel((context, options) =>
                {
                    options.Configure(_configuration.GetSection("Kestrel"));
                })
                .UseConfiguration(_configuration)
                .UseContentRoot(Environment.CurrentDirectory)                                                
                .UseSerilog()                
                .UseStartup<Startup>();
    }
}
