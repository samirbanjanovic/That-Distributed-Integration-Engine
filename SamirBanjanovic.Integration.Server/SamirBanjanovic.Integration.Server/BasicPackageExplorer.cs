using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnTrac.Integration.Server.Classes;
using OnTrac.Integration.Server.Core;

namespace OnTrac.Integration.Server
{
    public sealed class BasicPackageExplorer
        : IComponentPackageExplorer
    {
        private readonly ILogger<BasicPackageExplorer> _logger;        
        private readonly string _fileFilter;

        private FileSystemWatcher _packageWatcher;

        public BasicPackageExplorer(IOptions<PackageExplorerSettings> settings, ILogger<BasicPackageExplorer> logger)
        {
            _logger = logger;
            Settings = settings.Value;
            _fileFilter = $"*.{Settings.PackageExtension}";            
        }

        public PackageExplorerSettings Settings { get; }

        public IEnumerable<FileInfo> GetComponentPackagesToInstall()
        {
            string[] files;
            using (_logger.BeginScope("{@Settings}", Settings))
            {
                try
                {
                    files = Directory.GetFiles(Settings.PackageExtension, _fileFilter);

                    _logger.LogInformation("{Message}", $"Found {files.Length} package(s) for processing");                    
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{Message}", "Error during package exploration");
                    yield break;
                }
            }

            foreach (var file in files)
            {
                yield return new FileInfo(file);
            }
        }
        
        public void OnNewPackageAvailable(Func<FileInfo, Task> onPackageAvailableAction)
        {
            _packageWatcher?.Dispose();
            _packageWatcher = new FileSystemWatcher(Settings.PackagesDropDirectory, _fileFilter);
            _packageWatcher.Created += async (s, e) =>
            {
                _logger.LogInformation("{Message} {@Settings}", $"New package received - {e.FullPath}", Settings);
                await onPackageAvailableAction(new FileInfo(e.FullPath));
            };
            _packageWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            _packageWatcher.EnableRaisingEvents = Settings.ContinouslyMonitor;
        }
    }
}
