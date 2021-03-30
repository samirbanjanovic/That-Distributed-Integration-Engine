using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;

using OnTrac.Integration.Extensions.Logging;

namespace OnTrac.Integration.Components.FileWatcher
{
    // IGNORE NON-AWAIT ASYNC METHOD WARNINGS
    #pragma warning disable CS4014
    public sealed class FileWatcherComponent
        : IComponent
    {
        //8kb is the default buffer size per MS docs -  should allow for about 15 notificatoins
        //we shouldn't need to slow this down a queue should be used futher up the event chain
        private const int MINIMUM_BUFFER_SIZE = 8192;

        //64kb limit set by winapi
        //once more OSs are introduced change these consts to match the correct environment
        private const int MAXIMUM_BUFFER_SIZE = 65536;


        //context log for a given instance of FileEventNotifier
        private readonly ILogger<FileWatcherComponent> _logger;

        //buffer size received from configuration
        private int _bufferSize = 0;

        //.NET file system watcher responsible for monitoring
        //directories for incoming files
        private FileSystemWatcher _fileSystemWatcher;
        
        //publisher responsible for sending work 
        //to the next step in the pipeline
        private readonly IMessagePublisher _messagePublisher;
        private readonly ExistingFileChecker _existingFileChecker;

        // cancellation token used for managing existing file checker progress
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Monitors a directory for files that match a specified file filter, passed via
        /// properties.
        /// </summary>
        public FileWatcherComponent(IComponentSettings settings, IMessagePublisher eventPublisher, ILogger<FileWatcherComponent> logger)
        {
            Settings = settings;
            _logger = logger;
            _existingFileChecker = new ExistingFileChecker(this, _logger);
            _messagePublisher = eventPublisher;
            
            ConfigureDirectoryAndFilter();
            ConfigureBufferSize();
        }

        public string Path { get; private set; }
        public string Filter { get; private set; }

        public ObjectState State { get; set; }

        //name defaults to class name
        //if a name is given in configuration it's assigned 
        //during init
        public string Name => Settings.Name ?? nameof(FileWatcherComponent);
        
        //configuration for instance of notfier
        public IComponentSettings Settings { get; private set; }

        public Guid InstanceId { get; } = Guid.NewGuid();
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (_fileSystemWatcher == null)
                {
                    InitializeWatcher();
                }

                if (State == ObjectState.Stopped || State == ObjectState.Initialized)
                {
                    _cancellationTokenSource = new CancellationTokenSource();

                    BeginCheckForExistingFiles();

                    _fileSystemWatcher.EnableRaisingEvents = true;
                    State = ObjectState.Started;
                    _logger.LogInformation("{Message}", $"FileSystemWatcher started");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot start FileSystemWatcher when in {State} state");
                }
            }

            return Task.CompletedTask;
        }

        //stops the notifier from running and raising events
        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (_logger.ExtendLogScope(this))
            {
                if (State == ObjectState.Started)
                {
                    _cancellationTokenSource.Cancel();
                    _fileSystemWatcher.EnableRaisingEvents = false;
                    State = ObjectState.Stopped;
                    _logger.LogInformation("{Message}", $"FileSystemWatcher stopped");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Cannot stop FileSystemWatcher when in {State} state");
                }
            }
            return Task.CompletedTask;
        }

        // method to call when releaseing notifier
        public void Dispose()
        {
            using (_logger.ExtendLogScope(this))
            {
                DestroyCurrentInstanceFileSystemWatcher();
            }
        }

        //publish the event to the next level using IEventPublisher
        //that was injected into class
        internal async Task PublishNotificationAsync(params (string, string)[] itemsToAppend)
        {
            using (_logger.ExtendLogScope(this))
            {
                try
                {
                    var fileWatcherMessage = BuildMessageDetails(itemsToAppend);
                    _logger.LogInformation("{Message} {@ObjectProperties} {Correlation} {@ComponentMessage}", "Submitting items for publish", fileWatcherMessage.Properties, fileWatcherMessage.MessageId, fileWatcherMessage);

                    await _messagePublisher.PublishAsync(fileWatcherMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Message} {@ObjectProperties}", "Failed to submit items for publish", itemsToAppend?.ToArray());
                }
            }
        }

        //builds event details to be sent to publisher
        private IMessage BuildMessageDetails(params (string, string)[] propertiesToAdd)
        {

            var messageProperties = Settings.Properties.ToDictionary(x => x.Key, x => x.Value);
            foreach (var pair in propertiesToAdd)
            {
                messageProperties.Add(pair.Item1, pair.Item2);
            }

            IMessage fileMessage = new FileWatcherMessage
            {
                Source = Name,
                TimeStamp = DateTime.Now,
                Properties = messageProperties
            };
            return fileMessage;

        }
        
        //checks monitored directory for existing files
        //if files exist they will be published via PublishNotificationAsync(..)
        private async Task BeginCheckForExistingFiles()
        {
            using (_logger.ExtendLogScope(this))
            {
                try
                {                    
                    await _existingFileChecker.TraverseAsync(_cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message}", "Failed to check directory for existing files");
                }
            }
        }


        //intitialize watcher that will fire events
        //when files are available for processing...we hook onto the events
        //and send them to publisher for furhter notifications
        private void InitializeWatcher()
        {
            try
            {
                _fileSystemWatcher = new FileSystemWatcher(Path, Filter);

                if (_bufferSize > MINIMUM_BUFFER_SIZE && _bufferSize < MAXIMUM_BUFFER_SIZE)
                {
                    _fileSystemWatcher.InternalBufferSize = _bufferSize;
                    _logger.LogInformation("{Message}", $"InternalBufferSize set to {_bufferSize}");
                }
                else
                {
                    _logger.LogWarning("{Message}", $"Requested InternalBufferSize of {_bufferSize} is out of bounds (8kb > b < 64kb)");
                }

                _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;

                //subscribe to events via async 
                //this ensures the buffer is emptied as quickly as possible
                //and FileSystemWatcher threads are released in a reasonable timeframe
                _fileSystemWatcher.Created += async (s, e) => await FileCreatedEventHandlerAsync(s, e).ConfigureAwait(false);
                _fileSystemWatcher.Renamed += async (s, e) => await FileRenamedEventHandlerAsync(s, e).ConfigureAwait(false);
                _fileSystemWatcher.Error += ErroredWatcherStateReinitializationHandler;

                _logger.LogInformation("{Message}", "Successfully configured");

                State = ObjectState.Initialized;
            }
            catch (Exception exception)
            {
                State = ObjectState.Errored;
                _logger.LogError(exception, "{Message}", $"Failed to initialize");
            }

        }

        //release current instance of FileSystemWatcher in 
        //preperation for shutdown or creation of new instance
        private void DestroyCurrentInstanceFileSystemWatcher()
        {

            try
            {
                if (_fileSystemWatcher != null)
                {
                    _fileSystemWatcher.EnableRaisingEvents = false;
                    _fileSystemWatcher.Dispose();
                    _fileSystemWatcher = null;
                }

                State = ObjectState.Destroyed;
                _logger.LogInformation("{Message}", "Instance successfully disposed");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", "Failed to dispose of current instance");
                State = ObjectState.Errored;
            }

        }

        //handles FileSystemWatcher error...attempts to recover and restore to state prior to ERROR
        private void ErroredWatcherStateReinitializationHandler(object sender, ErrorEventArgs e)
        {
            using (_logger.ExtendLogScope(this))
            {
                Exception exception = e.GetException();
                try
                {
                    _logger.LogError(exception, "{Message}", "Internal error, destroying watcher instance");
                    DestroyCurrentInstanceFileSystemWatcher();
                    //TO DO: expand on the code and use polly to re-init the watcher
                }
                catch (Exception ex)
                {
                    State = ObjectState.Errored;
                    _logger.LogError(ex, "{Message}", "Failed to handle internal watcher error");
                }
            }
        }

        //manages file renamed event, if the new name  matches the desired filter 
        //the notification is published for consumption
        private async Task FileRenamedEventHandlerAsync(object sender, RenamedEventArgs e)
        {
            using (_logger.ExtendLogScope(this))
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(e.FullPath);

                    if (fileName.IsWildcardMatch(Filter))
                    {
                        await PublishNotificationAsync(("file", fileName), ("EventType", "ActionRequest")).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Message}", "Failed to initialize pipeline for ActionRequest");
                }
            }
        }

        //handles file created notification, by this point FileSystemWatcher has filtered 
        //for the correct name. Only thing left is to publish the notification 
        private async Task FileCreatedEventHandlerAsync(object sender, FileSystemEventArgs e)
        {
            using (_logger.ExtendLogScope(this))
            {
                try
                {
                    string fileName = System.IO.Path.GetFileName(e.FullPath);

                    await PublishNotificationAsync(("file", fileName), ("eventType", "ActionRequest")).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{Message}", "Failed to initialize pipeline for ActionRequest");
                }
            }
        }

        //reads directory and filter items to be used by FileSystemWatcher
        private void ConfigureDirectoryAndFilter()
        {
            if (!IsConfigurationValid())
            {
                _logger.LogError("{Message}", "Invalid configuraiton items - path and filter not part of dictionary");
                throw new KeyNotFoundException("\"path\" and \"filter\" are required configuration items");
            }

            Path = Settings.Properties["path"];
            Filter = Settings.Properties["filter"];

            if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Filter))
            {
                _logger.LogError("{Message}", "Invalid configuraiton items - path and filter must contain values");
                throw new ArgumentNullException("\"path\" and \"filter\" are required items");
            }
        }

        
        //reads buffer size
        private void ConfigureBufferSize()
        {
            if (Settings.Properties.TryGetValue("bufferSize", out string bufferSize))
            {
                if (int.TryParse(bufferSize, out int preAssignBufferCheck))
                {
                    _bufferSize = preAssignBufferCheck;
                }
            }
        }

        //checks if "path" and "filter" exist in the configuration       
        private bool IsConfigurationValid()
        {
            return Settings.Properties.ContainsKey("path") && Settings.Properties.ContainsKey("filter");
        }
    }
}
