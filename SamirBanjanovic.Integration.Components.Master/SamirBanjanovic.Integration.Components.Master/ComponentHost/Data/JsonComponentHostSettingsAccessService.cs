using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.Data.Entities;
using OnTrac.Integration.Components.Master.Node.Data.Entities;

namespace OnTrac.Integration.Components.Master.ComponentHost.Data
{
    public class JsonComponentHostSettingsAccessService
        : IComponentHostSettingsDataAccess
    {
        private readonly IEnumerable<Package> _componentPackages;
        private readonly ComponentHostInstanceSettings _componentMessagePublisher;
        private readonly IEnumerable<ComponentHostInstanceSettingsWithPublisher> _componentInstances;

        public JsonComponentHostSettingsAccessService()
        {

            var jsonText = File.ReadAllText("componentHost/data/componentSettings.json");
            var jObject = JObject.Parse(jsonText);

            _componentInstances = jObject["componentInstances"]
                                .Children()
                                .Select(x => x.ToObject<ComponentHostInstanceSettingsWithPublisher>())
                                .ToList();

            _componentPackages = jObject["componentPackages"]
                                .Children()
                                .Select(x => x.ToObject<Package>())
                                .ToList();                            
        }

        public Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync() 
            => Task.FromResult<IEnumerable<ComponentHostInstanceSettingsWithPublisher>>(_componentInstances.ToList());

        public Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync(NodeServer node)
            => Task.FromResult<IEnumerable<ComponentHostInstanceSettingsWithPublisher>>(_componentInstances.ToList());

        public Task<IEnumerable<Package>> GetComponentPackagesAsync()
            => Task.FromResult<IEnumerable<Package>>(_componentPackages.ToList());
        public Task<IEnumerable<Package>> GetComponentPackagesAsync(NodeServer node)
            => Task.FromResult<IEnumerable<Package>>(_componentPackages.ToList());
    }
}
