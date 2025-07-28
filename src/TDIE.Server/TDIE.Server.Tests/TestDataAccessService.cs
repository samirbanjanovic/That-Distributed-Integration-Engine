using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TDIE.Server.Core;

namespace TDIE.Server.Tests
{
    public class TestDataAccessService
        : IDataAccessService
    {
        private readonly string _writePath = @"C:\Apps\IntegrationPlatform\configurations";

        public Task RegisterResolvedComponentPackage(IComponentPackageConfiguration packageConfiguration)
        {
            var jsonSerializer = new JsonSerializer();
            var json = JsonConvert.SerializeObject(packageConfiguration);

            File.WriteAllText(Path.Combine(_writePath, packageConfiguration.GetType().ToString()), json);

            return Task.CompletedTask;
        }
    }
}
