using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.Data.Entities;
using OnTrac.Integration.Components.Master.Node.Data.Entities;

namespace OnTrac.Integration.Components.Master.ComponentHost.Data
{
    public interface IComponentHostSettingsDataAccess
    {
        Task<IEnumerable<Package>> GetComponentPackagesAsync();

        Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync();

        Task<IEnumerable<Package>> GetComponentPackagesAsync(NodeServer node);

        Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync(NodeServer node);
    }
}
