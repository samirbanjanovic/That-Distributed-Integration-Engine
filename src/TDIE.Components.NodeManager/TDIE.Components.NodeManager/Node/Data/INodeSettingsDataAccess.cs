using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TDIE.Components.NodeManager.Node.Data.Entities;
using TDIE.Components.NodeManager.Data.Entities;

namespace TDIE.Components.NodeManager.Node.Data
{
    public interface INodeSettingsDataAccess
    {
        Task<IEnumerable<NodeServer>> GetNodesAsync();

        Task<IEnumerable<Package>> GetPackagesAsync();

        Task<IEnumerable<Package>> GetPackagesAsync(NodeServer server);

        Task<string> GetComponentHostPackageName();
    }
}
