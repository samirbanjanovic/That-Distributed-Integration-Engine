using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TDIE.Components.FileWatcher;
using TDIE.Components.QuartzScheduler;
using TDIE.Components.WebApi;
using TDIE.Core;
using TDIE.Extensions.Logging;
using TDIE.Publishers.Basic;
using Serilog;


namespace TDIE.Tester
{
    static class Program
    {
        private static readonly ILoggerFactory _logFactory = new LoggerFactory()
                                                                    .AddSerilog();

        private readonly static IConfiguration _configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", false, true)
              .AddEnvironmentVariables()
              .Build();

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .Configure(_configuration)
                            .Enrich.WithProperty("LogicalName", "TesterService")
                            .CreateLogger();

            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine("Press enter to shut down");
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            Log.Information("{Message}", "Starting MainAsync()");

            var notifiers = new List<IComponent>();

            notifiers.AddRange(await TestWebHostExtensionAsync());
            notifiers.AddRange(await TestQuartzExtensionAsync());
            notifiers.AddRange(await TestWatcherExtensionAsync());

            Console.WriteLine("Press enter to start test");
            Console.ReadLine();

            foreach (var notifier in notifiers)
            {
                await notifier.StartAsync();
            }

            Console.WriteLine("Press enter to stop test");
            Console.ReadLine();

            foreach (var notifier in notifiers)
            {
                await notifier.StopAsync();
            }

            Console.WriteLine("Stopped!");
        }

        private static async Task<IEnumerable<IComponent>> TestWebHostExtensionAsync()
        {
            var webApis = new List<WebApiEndpointComponent>();

            var webApiConfig = new Dictionary<string, string>
            {
                { "type", "WebApi" },
                { "name", "AdHoc_Endpoint" },
                { "id", "1" },
                { "url", "http://localhost:9999"},
            };

            var webApi2Config = new Dictionary<string, string>
            {
                { "type", "WebApi" },
                { "name", "AdHoc_Endpoint2" },
                { "id", "1" },
                { "url", "http://localhost:9998"},
            };

            var basicPublisher = new TestPublisher(null, _logFactory.CreateLogger<TestPublisher>());
            await basicPublisher.StartAsync();

            var webApi = new WebApiEndpointComponent(webApiConfig, basicPublisher, _logFactory.CreateLogger<WebApiEndpointComponent>());
            var webApi2 = new WebApiEndpointComponent(webApi2Config, basicPublisher, _logFactory.CreateLogger<WebApiEndpointComponent>());

            webApis.Add(webApi);
            webApis.Add(webApi2);

            Console.WriteLine("Initialized WebApiEndpointExtension");

            return webApis;
        }


        private static async Task<IEnumerable<IComponent>> TestQuartzExtensionAsync()
        {
            var triggers = new List<QuartzSchedulerComponent>();

            var trigger1Config = new Dictionary<string, string>
            {
                { "type", "Trigger" },
                { "name", "TestTrigger_1" },
                { "id", "1" },
                { "cronSchedule", "0/5 * * * * ?" }
            };

            var trigger2Config = new Dictionary<string, string>
            {
                { "type", "Trigger" },
                { "name", "TestTrigger_2" },
                { "id", "2" },
                { "cronSchedule", "0/5 * * * * ?" }
            };

            var basicPublisher = new TestPublisher(null, _logFactory.CreateLogger<TestPublisher>());
            await basicPublisher.StartAsync();

            var trigger1 = new QuartzSchedulerComponent(trigger1Config, basicPublisher, _logFactory.CreateLogger<QuartzSchedulerComponent>());

            var trigger2 = new QuartzSchedulerComponent(trigger2Config, basicPublisher, _logFactory.CreateLogger<QuartzSchedulerComponent>());

            triggers.Add(trigger1);
            triggers.Add(trigger2);


            Console.WriteLine("Initialized QuartzSchedulerExtensions");

            return triggers;
        }

        private static async Task<IEnumerable<IComponent>> TestWatcherExtensionAsync()
        {
            var watchers = new List<FileWatcherComponent>();

            var watcher1Config = new Dictionary<string, string>
            {
                { "type", "Watcher" },
                { "name", "TestWatcher_1" },
                { "id", "1" },
                { "path", @"C:\drops\Test\Account\Inbound" },
                { "filter", @"*.test" },
                { "bufferSize", "1024" }
            };

            var watcher2Config = new Dictionary<string, string>
            {
                { "type", "Watcher" },
                { "name", "TestWatcher_2" },
                { "id", "2" },
                { "path", @"C:\drops\Test\IT Test" },
                { "filter", @"*.test" },
                { "bufferSize", "512" }
            };

            var basicPublisher = new TestPublisher(null, _logFactory.CreateLogger<TestPublisher>());
            await basicPublisher.StartAsync();

            var watcher1 = new FileWatcherComponent(watcher1Config, basicPublisher, _logFactory.CreateLogger<FileWatcherComponent>());
            var watcher2 = new FileWatcherComponent(watcher2Config, basicPublisher, _logFactory.CreateLogger<FileWatcherComponent>());

            watchers.Add(watcher1);
            watchers.Add(watcher2);

            Console.WriteLine("Initialized FileWatcherExtensions");


            return watchers;
        }
    }
}
