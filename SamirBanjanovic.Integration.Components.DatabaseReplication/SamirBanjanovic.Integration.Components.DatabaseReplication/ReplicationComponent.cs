using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.DatabaseReplication
{
    public class ReplicationComponent
        : IComponent
    {
        private readonly ILogger<ReplicationComponent> _logger;

        public ReplicationComponent(IComponentSettings settings, ILogger<ReplicationComponent> logger)
        {
            Settings = settings;
            _logger = logger;
        }

        public IComponentSettings Settings { get; }
        
        public string Name { get; }
        public Guid InstanceId { get; } = Guid.NewGuid();
        public ObjectState State { get; } = ObjectState.Initialized;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
