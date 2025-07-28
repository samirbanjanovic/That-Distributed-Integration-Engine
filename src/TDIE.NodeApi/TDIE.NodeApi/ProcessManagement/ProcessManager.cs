using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TDIE.NodeApi.Data;
using TDIE.NodeApi.Data.Entities;
using TDIE.NodeApi.Models;
using TDIE.Utilities.Mappers.Core;

namespace TDIE.NodeApi.ProcessManagement
{
    public class ProcessManager
        : IProcessManager
    {
        private readonly IProcessStoreAccess _storeAccess;
        private readonly IObjectMapperService _mapper;

        public ProcessManager(IProcessStoreAccess storeAccess, IObjectMapperService mapper)
        {
            _storeAccess = storeAccess;
            _mapper = mapper;
        }

        public async Task<bool> ProcessExistsAsync(Guid processGuid) => ProcessExists((await _storeAccess.GetNodeProcessesAsync(processGuid)).SystemProcessId);

        public async Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync() => _mapper.Map<ProcessDetailsModel, ProcessDetails>(await _storeAccess.GetNodeProcessesAsync());

        public async Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync(string packageName) => _mapper.Map<ProcessDetailsModel, ProcessDetails>(await _storeAccess.GetNodeProcessesAsync(packageName));

        public async Task<ProcessDetailsModel> GetNodeProcessesAsync(Guid nodeProcessId) => _mapper.Map<ProcessDetailsModel, ProcessDetails>(await _storeAccess.GetNodeProcessesAsync(nodeProcessId));

        public async Task<IEnumerable<ProcessDetailsModel>> GetNodeProcessesAsync(long settingsId) => _mapper.Map<ProcessDetailsModel, ProcessDetails>(await _storeAccess.GetNodeProcessesAsync(settingsId));

        private bool ProcessExists(int processId) => Process.GetProcesses().Any(x => x.Id == processId);


        public async Task KillProcessAsync(Guid processGuid)
        {
            ProcessDetails processDetails = await _storeAccess.GetNodeProcessesAsync(processGuid);

            if (processDetails is null || !ProcessExists(processDetails.SystemProcessId))
            {
                throw new Exception($"No process found with key {processGuid.ToString("N")}");
            }

            using (var process = Process.GetProcessById(processDetails.SystemProcessId))
            {
                process.Kill();

                await _storeAccess.DeleteProcessDetailsAsync(processDetails.NodeProcessId);
            }
        }

        public async Task<ProcessDetailsModel> StartProcessAsync(string packageName, string commandPath, (long Id, IDictionary<string, string> Args) instanceSettings)
        {
            using (var process = new Process())
            {
                var startInfo = new ProcessStartInfo()
                {
                    FileName = commandPath,
                    Arguments = string.Join(" ", instanceSettings.Args.Select(x => string.Join(" ", x.Key, x.Value))),
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    WorkingDirectory = Path.GetDirectoryName(commandPath)
                };

                process.StartInfo = startInfo;
                process.Start();

                return await SaveProcessDetailsAsync(packageName, commandPath, instanceSettings, process.Id);
            }
        }

        private async Task<ProcessDetailsModel> SaveProcessDetailsAsync(string packageName, string commandPath, (long Id, IDictionary<string, string> Args) instanceSettings, int processId)
        {
            var processDetails = new ProcessDetails()
            {
                PackageName = packageName,
                NodeProcessId = Guid.NewGuid(),
                Command = Path.GetFileName(commandPath),
                SettingsId = instanceSettings.Id,
                Arguments = instanceSettings.Args,
                SystemProcessId = processId,                
                StartDateTime = DateTime.Now, 
                ProcessUri = $"https://localhost:{instanceSettings.Args.FirstOrDefault(x => x.Key == "--port").Value}",
            };
                        
            await _storeAccess.InsertProcessDetailsAsync(processDetails);

            var processDetailsModel = _mapper.Map<ProcessDetailsModel, ProcessDetails>(processDetails);
            return processDetailsModel;
        }
    }
}
