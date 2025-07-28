using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TDIE.Server.Classes;
using TDIE.Server.Core;

namespace TDIE.Server.Tests
{
    public static class StaticResources
    {

        public static readonly IConfiguration Configuration = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", false, true)
                      .Build();

        public static readonly ServiceProvider ServiceProvider = new ServiceCollection()
                                .AddSingleton(Configuration)
                                .AddLogging()
                                .AddOptions()
                                .Configure<PlatformConfiguration>(Configuration.GetSection("Integration"))
                                .Configure<PackageInstallerSettings>(Configuration.GetSection("PackageInstaller"))
                                .Configure<PackageExplorerSettings>(Configuration.GetSection("PackageExplorer"))
                                .AddScoped<IDataAccessService, TestDataAccessService>()
                                .AddScoped<IComponentPackageInstaller, BasicPackageInstaller>()
                                .AddScoped<IComponentPackageExplorer, BasicPackageExplorer>()                                
                                .BuildServiceProvider();

    }

}
