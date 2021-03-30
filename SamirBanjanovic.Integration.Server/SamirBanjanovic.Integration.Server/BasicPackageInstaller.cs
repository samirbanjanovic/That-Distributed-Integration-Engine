using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnTrac.Integration.Server.Classes;
using OnTrac.Integration.Server.Core;

namespace OnTrac.Integration.Server
{
    public sealed class BasicPackageInstaller
        : IComponentPackageInstaller
    {
        private readonly IComponentStore _componentStoreAccess;
        private readonly ILogger<BasicPackageInstaller> _logger;


        public BasicPackageInstaller(IOptions<PackageInstallerSettings> settings, ILogger<BasicPackageInstaller> logger)
        {
            _logger = logger;
            Settings = settings.Value;
        }

        public PackageInstallerSettings Settings { get; }

        public async Task<IComponentMetadata> TryInstallPackageAsync(FileInfo package)
        {
            bool successfullyInstalledPackage = false;
            IComponentMetadata componentMetadata = null;
            try
            {
                // validate package path
                if (package == null || !package.Exists)
                {
                    _logger.LogError("{Message}", $"No package exist at path {package.FullName}");
                    return null;
                }

                // attempt to unzip the package and it's content for evaluation
                // at this point the package isn't installed but staged to ensure 
                // we don't move invalid packages to system and guarantee some kind of
                // stability for the server
                DirectoryInfo stagedPackageDirectory = await UnzipPackageAsync(package);

                using (_logger.BeginScope("{@Settings}", Settings))
                {
                    using (_logger.BeginScope("{PackageName}", package.Name))
                    {
                        if (stagedPackageDirectory != null && stagedPackageDirectory.Exists)
                        {
                            // read and evaluate the configuration file each package should have
                            // this is loaded from the staged path
                            if (TryLoadStagedPackageJsonConfiguration(stagedPackageDirectory, out IComponentPackageConfiguration packageConfiguration))
                            {
                                using (_logger.ExtendScopeWithPackageConfiguration(packageConfiguration))
                                {
                                    // with validation and staging complete we'll move the component folder and it's content 
                                    // to the servers component root directory
                                    componentMetadata = new ComponentMetadata
                                    {
                                        Id = Guid.NewGuid(),
                                        ComponentPackageConfiguration = packageConfiguration
                                    };

                                    if(await TryMovePackageFromStagingToInstall(stagedPackageDirectory, componentMetadata))                                    
                                    {
                                        _logger.LogInformation("{Message}", "Component package successfully installed");
                                        TryDeletePackageAndStagedDirectory(package, stagedPackageDirectory);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", "Unhandling packing installation exception encountered");
            }

            return componentMetadata;
        }

        private void TryDeletePackageAndStagedDirectory(FileInfo package, DirectoryInfo stagedPackageDirectory)
        {
            try
            {
                stagedPackageDirectory.Delete(true);
                package.Delete();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", $"Failed to delete package {package.Name} and staging directory {stagedPackageDirectory.Name}");
            }

        }

        private async Task<DirectoryInfo> UnzipPackageAsync(FileInfo package)
        {
            return await Task.Run((() =>
            {
                try
                {
                    string packageName = $"{Path.GetFileNameWithoutExtension(package.Name)}_{DateTime.Now.ToOADate()}";
                    DirectoryInfo destination = new DirectoryInfo(Path.Combine(Settings.StagingPath, packageName));

                    if (!destination.Exists)
                    {
                        destination.Create();
                        destination.Refresh();
                    }

                    ZipFile.ExtractToDirectory(package.FullName, destination.FullName);

                    return destination;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "{Message}", $"Failed to extract package at path {package}");

                    return null;
                }
            })).ConfigureAwait(false);
        }

        private async Task<bool> TryMovePackageFromStagingToInstall(DirectoryInfo stagedPackagePath, IComponentMetadata componentMetadata)
        {
            if (!IsValidatePackageConfiguration(componentMetadata.ComponentPackageConfiguration))
            {
                _logger.LogError("{Message}", "Package configuration requires valid Name and Version (#.#.#.#)");
                return false;
            }

            //create a folder path that represents a component package by version number
            //if we receive version 1.0.0.0 of a ComponentA, the resulting directory will be %componentRoot%/ComponentA.1.0.0.0            
            string installedComponentDirectory = Path.Combine(Settings.DestinationPath, $"{componentMetadata.ComponentPackageConfiguration.Name}.{componentMetadata.ComponentPackageConfiguration.Version}");

            if (Directory.Exists(installedComponentDirectory))
            {
                _logger.LogError("Component folder with the same version already exists");
                return false;
            }

            await stagedPackagePath.CopyToAsync(installedComponentDirectory);

            componentMetadata.ComponentDirectory = installedComponentDirectory;

            return true;
        }

        private bool TryLoadStagedPackageJsonConfiguration(DirectoryInfo stagedPackageDirectory, out IComponentPackageConfiguration componentPackageConfiguration)
        {
            componentPackageConfiguration = null;
            FileInfo stagedConfigurationFilePath = stagedPackageDirectory.GetFiles(Settings.InformationFile).FirstOrDefault();


            if (!stagedConfigurationFilePath.Exists)
            {
                _logger.LogError("{Message}", "Staged component package failed to install - no configuration file found");
                return false;
            }

            return TryDeserializeJsonConfiguration(stagedConfigurationFilePath, out componentPackageConfiguration);
        }

        private bool TryDeserializeJsonConfiguration(FileInfo configurationFile, out IComponentPackageConfiguration componentPackageConfiguration)
        {
            componentPackageConfiguration = null;
            try
            {
                using (var file = configurationFile.OpenText())
                {
                    var jsonSerializer = new JsonSerializer();
                    componentPackageConfiguration = (ComponentPackageConfiguration)jsonSerializer.Deserialize(file, typeof(ComponentPackageConfiguration));
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Message}", "Failed to deserialize json configuration file");

                return false;
            }
        }

        private static bool IsValidatePackageConfiguration(IComponentPackageConfiguration packageConfiguration)
        {
            return !string.IsNullOrEmpty(packageConfiguration.Name) && Version.TryParse(packageConfiguration.Version, out Version version) && string.IsNullOrEmpty(packageConfiguration.ComponentAssembly) && string.IsNullOrEmpty(packageConfiguration.FullyQualifiedComponentName);
        }


    }
}
