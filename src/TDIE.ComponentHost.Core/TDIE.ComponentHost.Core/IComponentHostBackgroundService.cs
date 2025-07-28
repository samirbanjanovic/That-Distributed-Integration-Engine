using System;
using System.Threading.Tasks;

using TDIE.Core;

namespace TDIE.ComponentHost.Core
{
    public interface IComponentHostBackgroundService
    {        
        Type ComponentType { get; }
        HostState HostState { get; }        
        IComponent Component { get; }
        IMessagePublisher MessagePublisher { get; }
        Type MessagePublisherType { get; }
        IServiceConfiguration ComponentConfiguration { get; }
        IServiceConfiguration MessagePublisherConfiguration { get; }
        void SetComponentConfiguration(IServiceConfiguration componentConfiguration);
        void SetMessagePublisherConfiguration(IServiceConfiguration messagePublisherConfiguration);

        Task Shutdown();
        Task<IComponentHostServiceResponse> ApplyConfigurationAsync();        
        Task<IComponentHostServiceResponse> StartAsync();
        Task<IComponentHostServiceResponse<IComponentSettings>> StartComponentAsync();
        Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StartMessagePublisherAsync();
        Task<IComponentHostServiceResponse> StopAsync();
        Task<IComponentHostServiceResponse<IComponentSettings>> StopComponentAsync();
        Task<IComponentHostServiceResponse<IMessagePublisherSettings>> StopMessagePublisherAsync();
    }
}