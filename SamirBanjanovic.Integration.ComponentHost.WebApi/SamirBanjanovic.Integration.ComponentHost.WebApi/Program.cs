using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnTrac.Integration.ComponentHost.WebApi.Extensions;
using OnTrac.Integration.Extensions.Logging;
using Polly;
using Polly.Retry;
using Serilog;

namespace OnTrac.Integration.ComponentHost.WebApi
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();

        private static readonly Guid _instanceId = Guid.NewGuid();

        // token source passed through layers to manage life cycle
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private static readonly IConfiguration _configuration;

        static Program()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            _configuration = new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();
        }


        // polly policy for handling errors and recovery
        // To Do: make this policy more robust so it resets
        // the entire state of the backgroudn worker to a new
        // un-errored instance
        private static AsyncRetryPolicy _policy;
        private static int _port;

        // counter used to keep track number of retries policy has attempted
        private static int _retries;

        private static bool _isConsole;
        private static bool _hasConsole;

        public static async Task<int> Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                            .Configure(_configuration)
                            .CreateLogger();

            try
            {
                CheckWindowsArguments(args);

                if (args.Any(x => x.ToLower() == "--port") && int.TryParse(args[Array.IndexOf(args, "--port") + 1], out int port))
                {
                    _port = port;
                }
                else
                {
                    throw new Exception("No port specified");
                }
                // use exponential back off with 5 attempts to
                // try and recover from an exception during  webhost.start or run
                _policy = CreateRetryPolicy();

                await _policy.ExecuteAsync(async () => await CreateWebHostBuilder().Build().RunAsync(_cancellationTokenSource.Token));

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{Message} {Correlation}", "WebApi host terminated unexpectedly", _instanceId);
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        private static void CheckWindowsArguments(string[] args)
        {
            if (SystemHelpers.IsWindows())
            {
                _isConsole = args.Any(x => x.ToLower() == "--console");
                if (_isConsole)
                {
                    // make a kernel32.dll call to detach application from console
                    // this will allow it to run in the background, even after closing
                    // instance of console it was called from

                    _hasConsole = AllocConsole();
                }
            }
        }



        public static AsyncRetryPolicy CreateRetryPolicy() => Policy.Handle<Exception>()
                        .WaitAndRetryAsync
                        (
                                retryCount: int.Parse(_configuration["Polly:AttemptLimit"]),
                                sleepDurationProvider: attempt => TimeSpan.FromMinutes(Math.Pow(int.Parse(_configuration["Polly:BackoffBase"]), attempt)), // back off exponentially in minutes
                                onRetry: (exception, timeSpan) =>
                                {
                                    _retries++;
                                    Log.Logger.Error(exception, "{Message}", $"Polly failed restart on retry {_retries}; backing off for {timeSpan.TotalMinutes} minute(s)");
                                    Log.CloseAndFlush();
                                }
                        );


        public static IWebHostBuilder CreateWebHostBuilder() => new WebHostBuilder()                
                .UseKestrel() 
                .UseUrls($"https://localhost:{_port}")
                .ConfigureKestrel((context, options) =>
                {                   
                    options.Configure(_configuration.GetSection("Kestrel"));                          
                })                
                .UseSerilog()                
                .UseConfiguration(_configuration)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(typeof(Guid), _instanceId)
                            .AddSingleton(_cancellationTokenSource);
                })
                .UseStartup<Startup>();        
    }
}
