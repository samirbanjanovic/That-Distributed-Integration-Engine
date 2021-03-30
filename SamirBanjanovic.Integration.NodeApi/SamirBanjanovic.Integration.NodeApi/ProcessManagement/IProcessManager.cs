using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnTrac.Integration.NodeApi.Models;

namespace OnTrac.Integration.NodeApi.ProcessManagement
{
    public interface IProcessManager
    {
        Task<ProcessDetailsModel> StartProcessAsync(string packageName, string commandPath, (long Id, IDictionary<string, string> Args) instanceSettings);

        Task KillProcessAsync(Guid processGuid);

        Task<bool> ProcessExistsAsync(Guid processGuid);

        Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync();

        Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync(string packageName);

        Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync(long settingsId);

        Task<ProcessDetailsModel> GetNodeProcessesAsync(Guid nodeProcessId);
    }
}
