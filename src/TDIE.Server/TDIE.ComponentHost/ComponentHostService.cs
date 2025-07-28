using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost._Core;
using TDIE.ComponentHost._Core.Interfaces;
using TDIE.ComponentHost.Classes;
using TDIE.ComponentHost.Helpers;
using TDIE.ComponentHost.Models;
using TDIE.Core;
using TDIE.Utilities.Mappers;
using TDIE.Utilities.Mappers.Core;

using static TDIE.ComponentHost.Helpers.StaticHelpers;

namespace TDIE.ComponentHost
{
    public class ComponentHostService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ComponentHostService> _logger;

        private IServiceProvider _serviceProvider;

        private IMessagePublisherSettings _messagePublisherSettings;

        private IComponentSettings _componentSettings;

        private IComponent _component;
        private IMessagePublisher _messagePublisher;

        public ComponentHostService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ComponentHostService>();

            HostState = HostState.AwaitingConfiguration;
        }

        public Guid HostId { get; } = Guid.NewGuid();

        public HostState HostState { get; private set; }

        public Type ComponentType { get; private set; }

        public Type MessagePublisherType { get; private set; }

        public ObjectState? ComponentState => _component?.State;

        public ObjectState? MessagePublisherState => _messagePublisher?.State;

        public ServiceConfigurationModel ServiceConfiguration { get; private set; }

        private async Task ConfigureServicesAsync()
        {

            await TerminateExistingInstancesAsync();

            //we can eventually turn this into an expression that can dynamically load all 
            //services that were requested - have a list of defaults, and load extra based on 
            //information sent accross via web request
            _serviceProvider = new ServiceCollection()
                     .AddSingleton(_loggerFactory)
                     .AddLogging()
                     .AddOptions()
                     .AddSingleton(_componentSettings)
                     .AddSingleton(_messagePublisherSettings)
                     .AddSingleton<IObjectMapperService, ObjectMapperService>()
                     .AddSingleton(typeof(IMessagePublisher), MessagePublisherType)
                     .AddSingleton(typeof(IComponent), ComponentType)
                     .BuildServiceProvider();

            _messagePublisher = _serviceProvider.GetService<IMessagePublisher>();
            _component = _serviceProvider.GetRequiredService<IComponent>();

            HostState = HostState.Configured;

            _logger.LogInformation("{Message} {@ObjectProperties)", "Configuration Success", this);
        }

        internal async Task<IComponentHostServiceResponse> ConfigureAsync(ServiceConfigurationModel serviceConfiguration)
        {
            var response = new ComponentHostServiceResponse();

            if (HostState == HostState.Started)
            {
                response.State = MemberState.Error;
                response.Message = "Services running - request Stop before changing configuration";
            }
            else
            {
                try
                {
                    await Task.Run(() => LoadRequestedAssemblies(serviceConfiguration));
                    await ConfigureServicesAsync();
                }
                catch (Exception e)
                {
                    string message = "Configuration failed";
                    _logger.LogError(e, "{Message} {@ObjectProperties}", message, this);
                    response.Set(MemberState.Error, message);
                    HostState = HostState.Errored;
                }
            }

            return response;
        }

        internal async Task<IComponentHostServiceResponse> StartAsync()
        {
            var response = new ComponentHostServiceResponse();
            if (HostState == HostState.Started)
            {
                return response;
            }
            else
            {
                try
                {
                    // message publisher is not null
                    // first start message publisher if there is one
                    // so that we are immediately publishing when component starts
                    if (!(_messagePublisher is null))
                    {
                        await _messagePublisher.StartAsync(CancellationToken.None);
                    }

                    // component is not null
                    if (!(_component is null))
                    {
                        await _component.StartAsync(CancellationToken.None);
                    }

                    HostState = HostState.Started;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start members", ServiceConfiguration);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, "Failed to start members");
                }
            }

            return response;
        }

        internal async Task<IComponentHostServiceResponse> StopAsync()
        {
            var response = new ComponentHostServiceResponse();
            if (HostState == HostState.Started)
            {
                try
                {
                    // message publisher is not null
                    // first start message publisher if there is one
                    // so that we are immediately publishing when component starts
                    if (!(_messagePublisher is null))
                    {
                        await _messagePublisher.StopAsync(CancellationToken.None);
                    }

                    // component is not null
                    if (!(_component is null))
                    {
                        await _component.StopAsync(CancellationToken.None);
                    }

                    HostState = HostState.Stopped;
                }
                catch (Exception e)
                {
                    string message = "Failed to Stop members";
                    _logger.LogError(e, "{Message} {@ObjectProperties}", message, ServiceConfiguration);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, message);
                }
            }
            else if (HostState != HostState.Stopped)
            {
                response.Set(MemberState.Error, $"Cannont Stop host when in {HostState.ToString()} state");
            }

            _logger.LogInformation("{Message} {@ObjectProperties}", $"Stop request processed -- response: {response.State}", response);

            return response;
        }

        internal async Task<IComponentHostServiceResponse<IComponentSettings>> StartComponentAsync()
        {
            var response = new ComponentHostServiceResponse<IComponentSettings>();

            if (_component is null)
            {
                response.Set(MemberState.Error, "Component not configured");
            }
            else
            {
                try
                {
                    await _component.StartAsync(CancellationToken.None);

                    response.Result = _componentSettings;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start Component", _componentSettings);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, "Component failed to Start");
                }
            }

            return response;
        }

        internal async Task<IComponentHostServiceResponse<IComponentSettings>> StopComponentAsync()
        {
            var response = new ComponentHostServiceResponse<IComponentSettings>();

            if (_component is null)
            {
                response.Set(MemberState.Error, "Component not configured");
            }
            else
            {
                try
                {
                    await _component.StopAsync(CancellationToken.None);

                    response.Result = _componentSettings;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Stop Component", _component);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, "Component failed to Stop");
                }
            }

            return response;
        }

        internal async Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StartMessagePublisherAsync()
        {
            var response = new ComponentHostServiceResponse<IMessagePublisherSettings>();
            if (_messagePublisher is null)
            {
                response.Set(MemberState.Error, "MessagePublisher not configured");
            }
            else
            {
                try
                {
                    await _messagePublisher.StartAsync(CancellationToken.None);

                    response.Result = _messagePublisherSettings;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start MessagePublisher", _component);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, "MessagePublisher failed to Start");
                }

            }

            return response;
        }

        internal async Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StopMessagePublisherAsync()
        {
            var response = new ComponentHostServiceResponse<IMessagePublisherSettings>();
            if (_messagePublisher is null)
            {
                response.Set(MemberState.Error, "MessagePublisher not configured");
            }
            else
            {
                try
                {
                    await _messagePublisher.StopAsync(CancellationToken.None);

                    response.Result = _messagePublisherSettings;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Stop MessagePublisher", _component);
                    HostState = HostState.Errored;
                    response.Set(MemberState.Error, "MessagePublisher failed to Stop");
                }
            }

            return response;
        }

        private void LoadRequestedAssemblies(ServiceConfigurationModel serviceConfiguration)
        {
            ServiceConfiguration = serviceConfiguration;

            ComponentType = GetTypeFromAssembly(serviceConfiguration.AssemblyPath, serviceConfiguration.FullyQualifiedClassName);

            if (ComponentType is null)
            {
                HostState = HostState.Errored;
            }
            else
            {
                ConfigureMessagePublisher(serviceConfiguration);
            }

            AssignMemberSettings(serviceConfiguration);
        }

        private void AssignMemberSettings(ServiceConfigurationModel serviceConfiguration)
        {
            if (HostState != HostState.Errored)
            {
                SetComponentSettings();
                SetMessagePubslisherSettings();
            }
            else
            {
                _logger.LogCritical("{Message} {@ObjectProperties}", "Assembly loading failure", serviceConfiguration);
            }

        }

        private void ConfigureMessagePublisher(ServiceConfigurationModel serviceConfiguration)
        {
            var usesMessagePublisher = ComponentType.UsesMessagePublisher();

            if (usesMessagePublisher && ServiceConfiguration.MessagePublisherConfiguration is null)
            {
                HostState = HostState.Errored;
            }
            else if (usesMessagePublisher)
            {
                MessagePublisherType = GetTypeFromAssembly(serviceConfiguration.MessagePublisherConfiguration.AssemblyPath, serviceConfiguration.MessagePublisherConfiguration.FullyQualifiedClassName);

                if (MessagePublisherType is null)
                {
                    HostState = HostState.Errored;
                }
            }
        }

        // add a timeout quit method that will cancel the request to an async method if the timeout is exceeded - 
        // this will be used for message publisher

        private async Task TerminateExistingInstancesAsync()
        {
            if (_component != null)
            {
                await StopComponentAsync();
            }

            if (_messagePublisher != null)
            {
                await StartComponentAsync();
            }

            _messagePublisher?.Dispose();
            _component?.Dispose();
        }

        private void SetComponentSettings()
        {
            _componentSettings = new ComponentSettings
            {
                Name = ServiceConfiguration.Name,
                Id = ServiceConfiguration.Id,
                Properties = ServiceConfiguration.Properties.ToDictionary(kv => kv.Key, kv => kv.Value)
            };
        }

        private void SetMessagePubslisherSettings()
        {
            if (ServiceConfiguration.MessagePublisherConfiguration != null)
            {
                _messagePublisherSettings = new MessagePublisherSettings
                {
                    Name = ServiceConfiguration.MessagePublisherConfiguration.Name,
                    Id = ServiceConfiguration.MessagePublisherConfiguration.Id,
                    Properties = ServiceConfiguration.MessagePublisherConfiguration.Properties.ToDictionary(kv => kv.Key, kv => kv.Value)
                };
            }
        }
    }
}
