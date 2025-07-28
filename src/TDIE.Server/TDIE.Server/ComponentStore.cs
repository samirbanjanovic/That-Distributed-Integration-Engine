using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TDIE.Server.Classes;
using TDIE.Server.Core;

namespace TDIE.Server
{
    public class ComponentStore
        : IComponentStore
    {
        private IDictionary<Guid, IComponentMetadata> _registeredComponents;

        private readonly ComponentStoreSettings _componentStoreConfiguration;
        private readonly ILogger<ComponentStore> _logger;

        public ComponentStore(IOptions<ComponentStoreSettings> settings, ILogger<ComponentStore> logger)
        {
            _componentStoreConfiguration = settings.Value;
            _logger = logger;
        }

        public IReadOnlyDictionary<Guid, IComponentMetadata> GetComponentMetadataTable() => _registeredComponents.ToDictionary(x => x.Key, x => x.Value);

        public Task AddOrUpdateComponentMetadataAsync(IComponentMetadata componentMetadata)
        {
            Guid key = componentMetadata.Id;

            if (_registeredComponents.ContainsKey(key))
            {
                _registeredComponents[key] = componentMetadata;
            }
            else
            {
                _registeredComponents.Add(key, componentMetadata);
            }

            CommitUpdateToDisk();

            return Task.CompletedTask;
        }

        private void CommitUpdateToDisk()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                Culture = CultureInfo.CurrentCulture
            };


            var jsonSerializer = JsonSerializer.Create();

            using (var jsonWriter = new StreamWriter(_componentStoreConfiguration.Location))
            {
                jsonSerializer.Serialize(jsonWriter, _registeredComponents);
            }
        }
    }
}
