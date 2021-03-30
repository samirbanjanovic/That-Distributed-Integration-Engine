using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.Master.ComponentHost.AccessService.Classes
{
    public class ComponentHostIntegrationExtensionInformation
    {
        public string Name { get; set; }

        public Guid InstanceId { get; set; }

        public string State { get; set; }
    }
}
