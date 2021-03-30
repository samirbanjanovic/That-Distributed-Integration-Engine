using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Server.Classes;
using OnTrac.Integration.Server.Core;

namespace OnTrac.Integration.Server
{
    public class Engine
        : IHostedService
    {
        private readonly ILogger<Engine> _logger;
        private readonly IPlatformConfiguration _platformConfiguration;
        private readonly IComponentPackageExplorer _componentPackageExplorer;
        private readonly IComponentPackageInstaller _componentPackageInstaller;        
        private readonly IComponentStore _componentStore;        

        private readonly IReadOnlyDictionary<Guid, IComponentMetadata> _registeredComponentPackageConfiguration;

        public Engine(IPlatformConfiguration platformConfiguration, IComponentPackageExplorer componentPackageExplorer
                     , IComponentPackageInstaller componentPackageInstaller, IComponentStore componentStore, ILogger<Engine> logger)
        {
            _logger = logger;
            _platformConfiguration = platformConfiguration;
                        
            _registeredComponentPackageConfiguration = _componentStore.GetComponentMetadataTable();
        }
                
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InstallNewPackages();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
     
        private async Task InstallNewPackages()
        {
            foreach (FileInfo packageToRegister in _componentPackageExplorer.GetComponentPackagesToInstall())
            {
                IComponentMetadata componentMetadata =  await _componentPackageInstaller.TryInstallPackageAsync(packageToRegister);
                
                await _componentStore.AddOrUpdateComponentMetadataAsync(componentMetadata);
            }
        }         
    }
}
