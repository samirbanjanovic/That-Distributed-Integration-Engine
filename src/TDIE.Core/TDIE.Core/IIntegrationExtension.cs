using System;
using Microsoft.Extensions.Hosting;

namespace TDIE.Core
{
    public interface IIntegrationExtension
        : IHostedService
        , IDisposable
    {
        string Name { get; }

        Guid InstanceId { get; }

        ObjectState State { get; }
    }
}
