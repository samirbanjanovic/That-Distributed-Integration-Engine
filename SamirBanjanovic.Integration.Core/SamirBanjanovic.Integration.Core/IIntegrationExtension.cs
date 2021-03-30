using System;
using Microsoft.Extensions.Hosting;

namespace OnTrac.Integration.Core
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
