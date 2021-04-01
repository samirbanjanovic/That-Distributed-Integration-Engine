using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Core;

namespace TDIE.Components.Master.ComponentHost.AccessService.Classes
{
    public class ComponentHostInformation
    {
        public Guid HostInstanceId { get; set; }
        public string HostState { get; set; }
        public string MessagePublisherType { get; set; }
        public string ComponentType { get; set; }
        public ComponentHostServiceConfigurationInformation ComponentConfiguration { get; set; }
        public ComponentHostServiceConfigurationInformation MessagePublisherConfiguration { get; set; }    
        public ComponentHostIntegrationExtensionInformation Component { get; set; }
        public ComponentHostMessagePublisherInformation MessagePublisher { get; set; }


    }
}
