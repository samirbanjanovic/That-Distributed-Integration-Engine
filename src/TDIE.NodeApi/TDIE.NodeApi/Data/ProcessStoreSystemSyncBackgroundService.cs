using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TDIE.NodeApi.Data;
using TDIE.NodeApi.Data.Entities;

namespace TDIE.NodeApi.ProcessManagement
{
    public class ProcessStoreSystemSyncBackgroundService
        : IHostedService
    {
        private readonly IProcessStoreAccess _processStoreAccess;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProcessStoreSystemSyncBackgroundService> _logger;
        private readonly double _checkFrequency;
        private Timer _checkTimer;

        public ProcessStoreSystemSyncBackgroundService(IProcessStoreAccess processStoreAccess, IConfiguration configuration, ILogger<ProcessStoreSystemSyncBackgroundService> logger)
        {
            _processStoreAccess = processStoreAccess;
            _configuration = configuration;
            _logger = logger;
            _checkFrequency = _configuration.GetValue<double>("Configuration:ProcessManager:CheckFrequency");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _checkTimer?.Dispose(); //dispose existing instance if possible
            _checkTimer = new Timer(CheckNodeProcesses, null, TimeSpan.FromMilliseconds(_checkFrequency), TimeSpan.FromMilliseconds(_checkFrequency));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _checkTimer?.Change(Timeout.Infinite, 0); // stop check timer - set delay to inifnite

            return Task.CompletedTask;
        }


        private async void CheckNodeProcesses(object state)
        {
            try
            {
                IEnumerable<ProcessDetails> registeredProcesses = await _processStoreAccess.GetNodeProcessesAsync();

                foreach (var process in registeredProcesses)
                {
                    if (!SystemProcessExists(process.SystemProcessId))
                    {
                        _logger.LogWarning("{Message} {@ObjectProperties}", "Zombie entry found in process store - killing zombie", process);
                        await _processStoreAccess.DeleteProcessDetailsAsync(process.NodeProcessId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", "Failed to sync store entries with system state");
            }
        }

        private bool SystemProcessExists(int processId)
        {
            return Process.GetProcesses().Any(x => x.Id == processId);
        }
    }
}
