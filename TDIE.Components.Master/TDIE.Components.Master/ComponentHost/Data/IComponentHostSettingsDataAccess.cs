using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.Data.Entities;
using TDIE.Components.Master.Node.Data.Entities;

namespace TDIE.Components.Master.ComponentHost.Data
{
    public interface IComponentHostSettingsDataAccess
    {
        Task<IEnumerable<Package>> GetComponentPackagesAsync();

        Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync();

        Task<IEnumerable<Package>> GetComponentPackagesAsync(NodeServer node);

        Task<IEnumerable<ComponentHostInstanceSettingsWithPublisher>> GetComponentInstanceSettingsAsync(NodeServer node);
    }
}
