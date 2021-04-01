using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.Classes;
using TDIE.ComponentHost.Core;
using TDIE.ComponentHost.Helpers;
using TDIE.Core;
using OnTrac.Utilities.Mappers;
using OnTrac.Utilities.Mappers.Core;
using static TDIE.ComponentHost.Helpers.StaticHelpers;

namespace TDIE.ComponentHost
{
    public sealed class ComponentHostBackgroundService
        : IComponentHostBackgroundService
    {
        //cancellation token source that is passed through 
        //the different layers to permit closing of application/system
        //via web call
        private readonly CancellationTokenSource _cancellationTokenSource;


        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ComponentHostBackgroundService> _logger;

        // used for creating new instance of configured Component and Message publisher
        private IServiceProvider _serviceProvider;

        // message publisher settings 
        // this is injected into message publisher from the service provider
        // when a new instance is requested
        private IMessagePublisherSettings _messagePublisherSettings;

        // component settings 
        // this is injected into message publisher from the service provider
        // when a new instance is requested
        // component settings are unique to each component, configuration is driven 
        // via a key|value pair -- this is very good component design and documentation
        // helps
        private IComponentSettings _componentSettings;

        public ComponentHostBackgroundService(Guid hostInstanceId, ILoggerFactory loggerFactory, CancellationTokenSource cancellationTokenSource)
        {
            HostInstanceId = hostInstanceId;
            _cancellationTokenSource = cancellationTokenSource;

            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ComponentHostBackgroundService>();

            HostState = HostState.AwaitingConfiguration;
        }

        // instance id received from upp layer -- this is further used to log exceptions
        // this way we can tie together all logs that belong to a single instance
        public Guid HostInstanceId { get; }

        // current state of background service
        public HostState HostState { get; private set; }

        // type of IComponent loaded after configuration -- if successful 
        public Type ComponentType { get; private set; }

        // type of IMessagePublisher loaded after configuration -- if successful 
        public Type MessagePublisherType { get; private set; }

        // the component that is hosted 
        public IComponent Component { get; private set; }

        // the optional message publisher hosted -- if component 
        // doesn't use a messsage publisher this will be null
        public IMessagePublisher MessagePublisher { get; private set; }

        // configuration used to set component 
        // this contains paths to assemblies and what settings each member will be injected
        // if this configuration is not correct init will fail
        public IServiceConfiguration ComponentConfiguration { get; private set; }

        // configuration used to set message publisher
        // this contains paths to assemblies and what settings each member will be injected
        // if this configuration is not correct init will fail
        public IServiceConfiguration MessagePublisherConfiguration { get; private set; }

        // configure internal services to be hosted -- this includes settings for component
        // and message publisher. on success reset state to configured
        private async Task ConfigureServicesCollectionAsync()
        {
            await TerminateExistingInstancesAsync();

            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
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

                MessagePublisher = _serviceProvider.GetService<IMessagePublisher>();
                Component = _serviceProvider.GetRequiredService<IComponent>();

                HostState = HostState.Configured;

                _logger.LogInformation("{Message} {@ObjectProperties})", "Configuration Success", this);
            }
        }

        public async Task Shutdown()
        {
            // dispose and clear component, message publisher, and service provider references
            // raise cancellation token -- this will shut down the web api and effectively terminate
            // the host
            await TerminateExistingInstancesAsync();

            _logger.LogInformation("{Message} {@ObjectProperties}", "Shutting down background service", this);

            _cancellationTokenSource.Cancel();
        }


        public void SetComponentConfiguration(IServiceConfiguration componentConfiguration)
        {
            if(HostState == HostState.AwaitingConfiguration)
            {
                ComponentConfiguration = componentConfiguration;
            }                        
        }
            


        public void SetMessagePublisherConfiguration(IServiceConfiguration messagePublisherConfiguration)
        {
            if (HostState == HostState.AwaitingConfiguration)
            {
                MessagePublisherConfiguration = messagePublisherConfiguration;
            }
        }        

        public async Task<IComponentHostServiceResponse> ApplyConfigurationAsync()
        {
            var response = new ComponentHostServiceResponse();

            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                // if we've already started swallow the request
                // and inform caller that we're already running
                // to re-configure services the host must be stopped -> configured -> started
                if (HostState == HostState.Started)
                {
                    response.State = MemberState.Error;
                    response.Message = "Services running - request Stop before changing configuration";
                }
                else
                {
                    try
                    {
                        LoadRequestedAssemblies();
                        await ConfigureServicesCollectionAsync();
                    }
                    catch (Exception e)
                    {
                        string message = "Configuration failed";
                        _logger.LogError(e, "{Message} {@ObjectProperties}", message, this);
                        response.Set(MemberState.Error, message);
                        HostState = HostState.Errored;
                    }
                }
            }

            return response;
        }

        // starts all hosted component and message publisher
        public async Task<IComponentHostServiceResponse> StartAsync()
        {
            var response = new ComponentHostServiceResponse();
            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
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
                        if (!(MessagePublisher is null))
                        {
                            await MessagePublisher.StartAsync(_cancellationTokenSource.Token);
                        }

                        // component is not null
                        if (!(Component is null))
                        {
                            await Component.StartAsync(_cancellationTokenSource.Token);
                        }

                        HostState = HostState.Started;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start members", ComponentConfiguration);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, "Failed to start members");
                    }
                }
            }

            return response;
        }

        // stops component and message publisher
        public async Task<IComponentHostServiceResponse> StopAsync()
        {
            var response = new ComponentHostServiceResponse();

            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                if (HostState == HostState.Started)
                {
                    try
                    {
                        // component is not null
                        // stop component first then message publisher 
                        // this way we publish any remaining messages
                        if (!(Component is null))
                        {
                            await Component.StopAsync(CancellationToken.None);
                        }

                        // message publisher is not null
                        // stop message publisher after component
                        // this way if message publishers have 
                        // any items in queue they can push them through
                        // in their stop method
                        if (!(MessagePublisher is null))
                        {
                            await MessagePublisher.StopAsync(CancellationToken.None);
                        }



                        HostState = HostState.Stopped;
                    }
                    catch (Exception e)
                    {
                        string message = "Failed to Stop members";
                        _logger.LogError(e, "{Message} {@ObjectProperties}", message, ComponentConfiguration);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, message);
                    }
                }
                else if (HostState != HostState.Stopped)
                {
                    response.Set(MemberState.Error, $"Cannont Stop host when in {HostState.ToString()} state");
                }

                _logger.LogInformation("{Message} {@ObjectProperties}", $"Stop request processed -- response: {response.State}", response);
            }
            return response;
        }

        // start component
        public async Task<IComponentHostServiceResponse<IComponentSettings>> StartComponentAsync()
        {
            var response = new ComponentHostServiceResponse<IComponentSettings>();

            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                if (Component is null)
                {
                    response.Set(MemberState.Error, "Component not configured");
                }
                else
                {
                    try
                    {
                        await Component.StartAsync(_cancellationTokenSource.Token);

                        response.Result = _componentSettings;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start Component", _componentSettings);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, "Component failed to Start");
                    }
                }
            }

            return response;
        }

        //stop component
        public async Task<IComponentHostServiceResponse<IComponentSettings>> StopComponentAsync()
        {
            var response = new ComponentHostServiceResponse<IComponentSettings>();
            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                if (Component is null)
                {
                    response.Set(MemberState.Error, "Component not configured");
                }
                else
                {
                    try
                    {
                        await Component.StopAsync(CancellationToken.None);

                        response.Result = _componentSettings;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Stop Component", Component);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, "Component failed to Stop");
                    }
                }
            }
            return response;
        }

        // start message publisher
        public async Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StartMessagePublisherAsync()
        {
            var response = new ComponentHostServiceResponse<IMessagePublisherSettings>();
            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                if (MessagePublisher is null)
                {
                    response.Set(MemberState.Error, "MessagePublisher not configured");
                }
                else
                {
                    try
                    {
                        await MessagePublisher.StartAsync(_cancellationTokenSource.Token);

                        response.Result = _messagePublisherSettings;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Start MessagePublisher", Component);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, "MessagePublisher failed to Start");
                    }

                }
            }

            return response;
        }

        // stop message publisher
        public async Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StopMessagePublisherAsync()
        {
            var response = new ComponentHostServiceResponse<IMessagePublisherSettings>();
            using (_logger.BeginScope("{Correlation}", HostInstanceId))
            {
                if (MessagePublisher is null)
                {
                    response.Set(MemberState.Error, "MessagePublisher not configured");
                }
                else
                {
                    try
                    {
                        await MessagePublisher.StopAsync(CancellationToken.None);

                        response.Result = _messagePublisherSettings;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "{Message} {@ObjectProperties}", "Failed to Stop MessagePublisher", Component);
                        HostState = HostState.Errored;
                        response.Set(MemberState.Error, "MessagePublisher failed to Stop");
                    }
                }
            }

            return response;
        }

        // attempt to load all assemblies passed through configuration
        private void LoadRequestedAssemblies()
        {
            // this is when the validity of the configuration will be important
            // if paths or namespaces are incorrect we won't be able to load 
            // the requested resources, resulting in an error state
            ComponentType = GetTypeFromAssembly(ComponentConfiguration.AssemblyPath, ComponentConfiguration.FullyQualifiedClassName);

            // component is required, while message publisher is optional
            // since not all components will use a message publisher
            if (ComponentType is null)
            {
                HostState = HostState.Errored;
            }
            else if (!(MessagePublisherConfiguration is null))
            {
                ConfigureMessagePublisher();
            }

            _componentSettings = ComponentConfiguration.ToComponentSettings();
            _messagePublisherSettings = MessagePublisherConfiguration.ToMessagePublisherSettings();
        }

        private void ConfigureMessagePublisher()
        {
            //if a component is expecting a message publisher 
            var usesMessagePublisher = ComponentType.UsesMessagePublisher();

            if (usesMessagePublisher && MessagePublisherConfiguration is null)
            {
                // compent is expecting a message publisher but we received no information on how to load one
                throw new NullReferenceException($"Component of type {ComponentType} is expecting an IMessagePublisher -- an implementation was not provided");
            }
            else if (usesMessagePublisher)
            {
                // attempt to load message publisher
                MessagePublisherType = GetTypeFromAssembly(MessagePublisherConfiguration.AssemblyPath, MessagePublisherConfiguration.FullyQualifiedClassName);

                if (MessagePublisherType is null)
                {
                    throw new NullReferenceException($"Component of type {ComponentType} is expecting an IMessagePublisher -- received invalid configuration");
                }
            }
        }


        private async Task TerminateExistingInstancesAsync()
        {
            if (Component != null)
            {
                await StopComponentAsync();
            }

            if (MessagePublisher != null)
            {
                await StopMessagePublisherAsync();
            }

            MessagePublisher?.Dispose();
            Component?.Dispose();

            HostState = HostState.Destroyed;

            _serviceProvider = null;
        }
    }
}

