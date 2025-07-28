using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using TDIE.Extensions.Logging;
using TDIE.Utilities.Mappers;
using TDIE.Utilities.Mappers.Core;
using Serilog;


namespace TDIE.ComponentHost
{
    public class Program
    {
        private static readonly IConfiguration _configuration = new ConfigurationBuilder()
                                                                      .SetBasePath(Directory.GetCurrentDirectory())
                                                                      .AddJsonFile("appsettings.json", false, true)
                                                                      .Build();

        private static ILoggerFactory _loggerFactory;

        // class doing all the work in the background
        private static ComponentHostService _componentHostService;

        static async Task Main(string[] args)
        {
            Uri uri = null;

            Log.Logger = new LoggerConfiguration()
                             .Configure(_configuration)
                             .CreateLogger();

            if (args.Length == 0 || args[0] != "--url")
            {
                LogFatalAndCloseFlush($"incorrect console arguments", args);
                throw new ArgumentException("incorrect console arguments; expecting --url {url}");
            }
            else if(string.IsNullOrEmpty(args[1]))
            {
                LogFatalAndCloseFlush($"--url flag specified, but no url given", args);
                throw new ArgumentException("--url flag specified, but no url given");                
            }
            else if (!Uri.TryCreate(args[1], UriKind.Absolute, out uri))
            {
                LogFatalAndCloseFlush($"failed to resolve url", args);
                throw new ArgumentException($"failed to resolve url - \"{args[1]}\"");
            }

            _loggerFactory = new LoggerFactory().AddSerilog(dispose: true);
            _componentHostService = new ComponentHostService(_loggerFactory);

            // this blocks until CTRL + C is pressed or app is shut down
            await RunWebInterfaceHost(uri);
        }

        private static void LogFatalAndCloseFlush(string message, string[] args)
        {            
            Log.Logger.Fatal("{SourceContext} {Message} {@ObjectProperties}", "TDIE.ComponentHost.Program", message, args);
            Log.CloseAndFlush();
        }

        private static async Task RunWebInterfaceHost(Uri uri)
        {           
            try
            {
                // create a new IHostedService/BackgroundService class that
                // interfaces with ComponentHostCore - the hosted service will receive
                // a request that is enqued for processing 

                var webApiHost = new WebHostBuilder()
                                .UseKestrel()
                                .UseUrls(uri.AbsoluteUri)
                                .UseSerilog()
                                .ConfigureServices(services =>
                                {
                                    services.AddOptions()
                                            .AddSingleton(_loggerFactory)
                                            .AddLogging()
                                            // implement this interface in such a way that 
                                            // all api requests are sent to it, which in return 
                                            // performs work on ComponetHostCore
                                            .AddSingleton<IObjectMapperService, ObjectMapperService>()
                                            .AddSingleton(_componentHostService)
                                            .AddMvc()
                                            .AddJsonOptions(options =>
                                            {
                                                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                                            });
                                })
                                .Configure(app =>
                                {
                                    app.UseMvc();
                                })
                                .Build();

                Log.Logger.Information("{Message}", $"Webhost started. Listening url \"{uri.ToString()}\"");

                await webApiHost.RunAsync();
            }
            catch (Exception exception)
            {
                Log.Logger.Error(exception, "{Message}", "Failed to initialize WebHost");
                Log.CloseAndFlush();
                throw;
            }
        }

    }
}
