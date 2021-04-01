using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDIE.NodeApi.Data.Entities;

namespace TDIE.NodeApi.Data
{
    public interface IProcessStoreAccess
    {
        Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync();

        Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync(string packageName);

        Task<IEnumerable<ProcessDetails>> GetNodeProcessesAsync(long settingsId);

        Task<ProcessDetails> GetNodeProcessesAsync(Guid processId);

        Task InsertProcessDetailsAsync(ProcessDetails processDetails);
        
        Task DeleteProcessDetailsAsync(Guid processId);
    }
}
