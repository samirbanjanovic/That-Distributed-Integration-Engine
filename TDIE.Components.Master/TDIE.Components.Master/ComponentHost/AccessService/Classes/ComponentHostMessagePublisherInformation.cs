using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.ComponentHost.AccessService.Classes
{
    public class ComponentHostMessagePublisherInformation
        : ComponentHostIntegrationExtensionInformation
    {
        public class ComponentHostMessagePublisherSettingsInformation
        {
            public string Name { get; set; }

            public long Id { get; set; }

            public IDictionary<string, string> Properties { get; set; }
        }

        public ComponentHostMessagePublisherSettingsInformation Settings { get; set; }
    }
}
