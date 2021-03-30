using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OnTrac.Integration.Components.Master.Node.Data.Entities;
using OnTrac.Integration.Components.Master.Data.Entities;

namespace OnTrac.Integration.Components.Master.Node.Data
{
    public interface INodeSettingsDataAccess
    {
        Task<IEnumerable<NodeServer>> GetNodesAsync();

        Task<IEnumerable<Package>> GetPackagesAsync();

        Task<IEnumerable<Package>> GetPackagesAsync(NodeServer server);

        Task<string> GetComponentHostPackageName();
    }
}
