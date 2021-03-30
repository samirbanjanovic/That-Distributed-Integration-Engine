using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace OnTrac.Integration.Components.FileWatcher
{
    public class ExistingFileChecker
    {
        private readonly ILogger _logger;
        private readonly FileWatcherComponent _fileEventNotifier;
        private readonly Guid _instanceId = Guid.NewGuid();

        public ExistingFileChecker(FileWatcherComponent fileWatcherComponent, ILogger logger)
        {
            _logger = logger;
            _fileEventNotifier = fileWatcherComponent;
        }
        
        // checks directory for existing files using same filter as FileEventNotifier
        // for each file found the OnActionRequired method is called on it's own 
        // worker thread
        public async Task TraverseAsync(CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("{ResourceType} {Correlation}", nameof(ExistingFileChecker), _instanceId))
            {
                string[] files = Directory.GetFiles(_fileEventNotifier.Path, _fileEventNotifier.Filter);

                if (files.Any())
                {
                    _logger.LogInformation("{Message}", $"Found existing files for processing. Count: {files.Length}");
                    foreach (var file in files)
                    {
                        try
                        {
                            if(cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }

                            await _fileEventNotifier.PublishNotificationAsync(("file", file), ("eventType", "ActionRequest")).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception, "{Message} {Correlation}", $"Failed to submit publish request for \"{file}\"", _instanceId);
                        }
                    }
                }
            }
        }
    }
}
