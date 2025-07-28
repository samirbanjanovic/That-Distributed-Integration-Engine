using System.Linq;
using TDIE.ComponentHost.Classes;
using TDIE.ComponentHost.Core;

using TDIE.Core;

namespace TDIE.ComponentHost.Helpers
{
    public static class ServiceConfigurationExtensions
    {
        public static IComponentSettings ToComponentSettings(this IServiceConfiguration configuration)
        {
            var settings = new ComponentSettings
            {
                Name = configuration.Name,
                Properties = configuration.Properties.ToDictionary(kv => kv.Key, kv => kv.Value)
            };

            if (settings.Properties.TryGetValue("id", out string id) && long.TryParse(id, out long numericId))
            {
                settings.Id = numericId;
            }

            return settings;
        }


        public static IMessagePublisherSettings ToMessagePublisherSettings(this IServiceConfiguration configuration)
        {
            var settings = new MessagePublisherSettings
            {
                Name = configuration.Name,
                Properties = configuration.Properties.ToDictionary(kv => kv.Key, kv => kv.Value)
            };

            if (settings.Properties.TryGetValue("id", out string id) && long.TryParse(id, out long numericId))
            {
                settings.Id = numericId;
            }

            return settings;
        }

    }
}
