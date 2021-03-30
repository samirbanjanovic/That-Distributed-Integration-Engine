using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;
using OnTrac.Integration.Extensions.Logging;

namespace OnTrac.Integration.Publishers.Basic
{
    public class TestPublisher
        : IMessagePublisher
    {
        private readonly ILogger<TestPublisher> _logger;

        private readonly object _lock = new object();


        public TestPublisher(IMessagePublisherSettings settings, ILogger<TestPublisher> logger)
        {
            Settings = settings;
            _logger = logger;
            
            State = ObjectState.Initialized;
        }

        public IMessagePublisherSettings Settings { get; }

        public string Name => Settings.Name ?? nameof(TestPublisher);

        public ObjectState State { get; private set; }

        public Guid InstanceId { get; } = Guid.NewGuid();

        public IReadOnlyDictionary<string, string> Properties { get; }

        public async Task PublishAsync(IMessage eventDetails)
        {
            await Task.Run(() => PublishEnqueuedEvent(eventDetails)).ConfigureAwait(false);
        }

        public Task Configure()
        {
            State = ObjectState.Initialized;
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            State = ObjectState.Started;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            State = ObjectState.Stopped;
            return Task.CompletedTask;
        }

        private void PublishEnqueuedEvent(IMessage message)
        {
            using (_logger.BeginScope("{ObjectProperties} {Name} {Correlation} {ResourceType}", Settings.Properties, Name, InstanceId, "MessagePublisher"))
            {
                if (State == ObjectState.Started)
                {
                    try
                    {
                        _logger.LogInformation("{Message} {Correlation} {@ComponentMessage}", "Received event, generating request", message.MessageId, message);
                        Thread.Sleep(1000);
                        if (message.Properties["type"] == "Watcher")
                        {
                            System.IO.File.Delete(Path.Combine(message.Properties["path"], message.Properties["file"]));
                        }

                        _logger.LogInformation("{Message} {Correlation} {@ComponentMessage}", "Request forwarded", message.MessageId, message);

                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "{Message} {Correlation} {@ComponentMessage}", "Failed to publish request", message?.MessageId, message);
                    }

                }            
            }
        }

        public void Dispose()
        {
            
        }
    }
}
