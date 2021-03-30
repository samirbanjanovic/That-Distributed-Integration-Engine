using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.NodeApi.Extensions;
using OnTrac.Integration.NodeApi.Models;
using OnTrac.Integration.NodeApi.ProcessManagement;
using OnTrac.Integration.PackageManager.Core;

namespace OnTrac.Integration.NodeApi.Controllers
{
    [Route("api/node/processes")]
    [ApiController]
    public class ProcessesController : ControllerBase
    {
        private readonly IPackageManager _packageManager;
        private readonly IProcessManager _processManager;

        private readonly ILogger<ProcessesController> _logger;

        public ProcessesController(IPackageManager packageManager, IProcessManager processManager, ILogger<ProcessesController> logger)
        {
            _packageManager = packageManager;
            _processManager = processManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPackageInstances()
        {
            return Ok(await _processManager.GetNodeProcessesAsync());
        }

        [HttpGet("{packageName:required:minlength(1)}")]
        public async Task<IActionResult> GetPackageInstances(string packageName)
        {
            return Ok(await _processManager.GetNodeProcessesAsync(packageName));
        }

        [HttpGet("{packageName:required:minlength(1)}/{processId:guid:required}")]
        public async Task<IActionResult> GetPackageInstance(string packageName, Guid processId)
        {
            return Ok(await _processManager.GetNodeProcessesAsync(processId));
        }

        [HttpGet("{packageName:required:minlength(1)}/{settingsId:long:required}")]
        public async Task<IActionResult> GetPackageInstance(string packageName, long settingsId)
        {
            return Ok(await _processManager.GetNodeProcessesAsync(settingsId));
        }

        [HttpPut("{packageName:required:minlength(1)}/start")]
        public async Task<IActionResult> StartPackageInstance(string packageName, [FromBody](long id, IDictionary<string, string> args) instanceSettings)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                IPackageConfiguration packageConfiguration = await _packageManager.GetPackageConfigurationAsync(packageName);
                string commandPath = packageConfiguration.GetCommandPath(_packageManager.PackageRoot);

                ProcessDetailsModel processDetails = await _processManager.StartProcessAsync(packageName, commandPath, instanceSettings);

                return processDetails is null ? Ok(null) : Ok(processDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to start package");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPut("{packageName:required:minlength(1)}/{processId:guid:required}/kill")]
        public async Task<IActionResult> KillPackageInstances(string pacakgeName, Guid processId)
        {
            try
            {
                await _processManager.KillProcessAsync(processId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to stop process {processId}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("kill")]
        [HttpPut("{packageName:required:minlength(1)}/kill")]
        public async Task<IActionResult> KillProcesses(string packageName = null)
        {
            try
            {
                IEnumerable<ProcessDetailsModel> processes;

                if (string.IsNullOrEmpty(packageName))
                {
                    processes = await _processManager.GetNodeProcessesAsync();
                }
                else
                {
                    processes = await _processManager.GetNodeProcessesAsync(packageName);
                }

                await KillProcessListAsync(processes);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to kill processes");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task KillProcessListAsync(IEnumerable<ProcessDetailsModel> processDetails)
        {
            await Task.Run(async () =>
            {
                foreach (var process in processDetails)
                {
                    try
                    {
                        await _processManager.KillProcessAsync(process.NodeProcessId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{Message} {@ObjectProperties}", $"Failed to kill processes", process);
                    }
                }

            });
        }
    }
}