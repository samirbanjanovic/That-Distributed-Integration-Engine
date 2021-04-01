using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TDIE.Components.Master.Classes;
using TDIE.Components.Master.Node.AccessService.Classes;
using TDIE.Components.Master.Node.Data.Entities;


namespace TDIE.Components.Master.Node.AccessService
{
    public interface INodeAccessService
    {
        NodeServer ClientNode { get; }

        #region package management

        Task<IEnumerable<PackageDetails>> GetNodePackageConfigurationAsync();

        Task<PackageDetails> GetNodePackageConfigurationAsync(string packageName);

        Task<PackageDetails> UploadPackageAsync(Stream package);

        Task<PackageDetails> UpdatePackageAsync(Stream Package);

        #endregion package management

        #region node stats

        Task<NodeSystemStats> GetNodeStatsAsync();

        Task<IEnumerable<NodeProcessInformation>> GetNodeProcessStatsAsync();

        Task<IEnumerable<NodeProcessInformation>> GetNodeProcessStatsAsync(string packageName);

        #endregion node stats

        #region process management

        Task<NodeBasicProcessInformation> StartProcessAsync(string packageName, (long id, IDictionary<string, string> args) processArgs);

        Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync();

        Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync(string packageName);

        Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync(string packageName, long settingsId);

        Task<NodeBasicProcessInformation> GetPackageProcessInstanceAsync(string packageName, Guid nodeProcessId);

        

        Task<bool> KillProcessAsync(string packageName, Guid nodeProcessId);

        Task<bool> KillProcessesAsync(string packageName);

        Task<bool> KillProcessesAsync();
        #endregion process management

    }
}
