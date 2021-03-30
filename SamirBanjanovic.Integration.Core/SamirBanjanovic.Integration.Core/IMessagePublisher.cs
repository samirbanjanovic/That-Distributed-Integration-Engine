using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OnTrac.Integration.Core
{
    public interface IMessagePublisher
        : IIntegrationExtension
    {
        IMessagePublisherSettings Settings { get; }

        Task PublishAsync(IMessage message);
    }
}
