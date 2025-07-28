using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TDIE.Components.NodeManager.Data.Entities;
using TDIE.Components.NodeManager.Node.Data.Entities;

namespace TDIE.Components.NodeManager.Node.Data
{
    public class JsonNodeSettingsAccessService
        : INodeSettingsDataAccess
    {
        private readonly IEnumerable<NodeServer> _nodeServers;
        private readonly IEnumerable<Package> _nodePackages;
        private readonly string _componentHostPackageName;

        public JsonNodeSettingsAccessService()
        {
            
            var jsonText = File.ReadAllText("node/data/clusterSettings.json");
            var jObject = JObject.Parse(jsonText);

            _nodeServers = jObject["nodeServers"]
                                .Children()
                                .Select(x => x.ToObject<NodeServer>())
                                .ToList();

            _nodePackages = jObject["nodePackages"]
                                .Children()
                                .Select(x => x.ToObject<Package>())
                                .ToList();

            _componentHostPackageName = jObject["componnetHostPackageName"].Value<string>();
        }

        public Task<string> GetComponentHostPackageName() => Task.FromResult<string>(_componentHostPackageName);
        public Task<IEnumerable<NodeServer>> GetNodesAsync() => Task.FromResult<IEnumerable<NodeServer>>(_nodeServers.ToList());


        public Task<IEnumerable<Package>> GetPackagesAsync() => Task.FromResult<IEnumerable<Package>>(_nodePackages.ToList());

        public Task<IEnumerable<Package>> GetPackagesAsync(NodeServer server) => Task.FromResult<IEnumerable<Package>>(_nodePackages.ToList());
    }
}
