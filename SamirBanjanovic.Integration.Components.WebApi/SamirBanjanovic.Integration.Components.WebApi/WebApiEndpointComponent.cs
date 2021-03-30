using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;
using OnTrac.Integration.Extensions.Logging;
using Serilog;

namespace OnTrac.Integration.Components.WebApi
{
    // work in progress
    public class WebApiEndpointComponent
        : IComponent
    {
        private readonly IMessagePublisher _messagePublisher;

        private readonly ILogger<WebApiEndpointComponent> _logger;

        private IWebHost _webHost;
        
        public WebApiEndpointComponent(IComponentSettings settings, IMessagePublisher messagePublisher, ILogger<WebApiEndpointComponent> logger)
        {
            Settings = settings;
            _logger = logger;

            
            _messagePublisher = messagePublisher;
            
            ConfigureUrl();
        }

        public string Url { get; private set; }

        public IComponentSettings Settings { get; }
        public string Name => Settings.Name ?? nameof(WebApiEndpointComponent);
        public Guid InstanceId { get; } = Guid.NewGuid();
        public ObjectState State { get; private set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (_webHost == null)
                {
                    InitializeWebHost();
                }

                if (State == ObjectState.Stopped || State == ObjectState.Initialized)
                {                    
                    await _webHost.StartAsync(cancellationToken).ConfigureAwait(false);
                    State = ObjectState.Started;
                    _logger.LogInformation("{Message}", $"Webhost started");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot start WebHost when in {State} state");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (State == ObjectState.Started)
                {
                    using (_logger.ExtendLogScope(this))
                    {
                        await DestroyCurrentWebHostInstance();
                    }

                    _logger.LogInformation("{Message}", "WebApiHost stopped");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot stop WenHost when in {State} state");
                }
            }
        }

        public void Dispose()
        {
            using (_logger.ExtendLogScope(this))
            {                
                DestroyCurrentWebHostInstance()                
                        .GetAwaiter()
                        .GetResult();

                State = ObjectState.Destroyed;
            }
        }

        private async Task DestroyCurrentWebHostInstance()
        {
            if(_webHost is null)
            {
                return;
            }

            try
            {
                await _webHost.StopAsync(CancellationToken.None);
                _webHost.Dispose();
                _webHost = null;
                _logger.LogInformation("{Message}", "WebHost disposed");
                State = ObjectState.Stopped;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", "Failed to dispose of current instance");
                State = ObjectState.Errored;
            }
        }


        private void ConfigureUrl()
        {
            if (Settings.Properties.TryGetValue("url", out string url))
            {
                Url = url;
            }
            else
            {
                Url = "http://localhost:9337";
            }
        }

        private void InitializeWebHost()
        {
            try
            {
                var webHostBuilder = new WebHostBuilder()
                                            .UseKestrel()
                                            .UseUrls(Url)
                                            .UseSerilog()
                                            .ConfigureServices(services =>
                                            {
                                                services.AddOptions()
                                                        .AddLogging(config => config.AddSerilog(dispose: true))
                                                        .AddSingleton<IComponent>(this)
                                                        .AddSingleton<IMessagePublisher>(_messagePublisher)
                                                        .AddMvc();
                                            })
                                            .Configure(app => app.UseMvc());
                                
                               
                _webHost = webHostBuilder.Build();
                State = ObjectState.Initialized;

                _logger.LogInformation("{Message}", $"Webhost configured for url \"{Url}\"");
            }
            catch (Exception exception)
            {
                State = ObjectState.Errored;
                _logger.LogCritical(exception, "{Message}", "Failed to initialize WebHost");
            }
        }
    }
}
