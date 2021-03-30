using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.NodeApi.Extensions;
using OnTrac.Integration.NodeApi.ProcessManagement;
using OnTrac.Integration.PackageManager.Core;

namespace OnTrac.Integration.NodeApi.Controllers
{
    [Route("api/node")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly IPackageManager _packageManager;
        private readonly IProcessManager _processManager;

        private readonly ILogger<NodeController> _logger;

        public NodeController(IPackageManager packageManager, IProcessManager processManager, ILogger<NodeController> logger)
        {
            _packageManager = packageManager;
            _processManager = processManager;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var framework = Assembly
                    .GetEntryAssembly()?
                    .GetCustomAttribute<TargetFrameworkAttribute>()?
                    .FrameworkName;

                var packageInstanceDetails = (await _processManager.GetNodeProcessesAsync())
                    .Select(pdm => pdm.ToPackageInstanceDetails())
                    .GroupBy(pid => new { pid.PackageName, pid.Command })
                    .Select(pg => new
                    {
                        pg.Key.PackageName,
                        pg.Key.Command,
                        Count = pg.Count(),
                        TotalWorkingSet64 = pg.Sum(x => x.WorkingSet64),
                        AverageWorkingSet64 = pg.Average(x => x.WorkingSet64),
                        TotalProcessorTimeInSeconds = pg.Sum(x => x.ProcessorTimeInSeconds),
                        AverageProcessorTimeInSeconds = pg.Average(x => x.ProcessorTimeInSeconds),
                        Instances = pg.ToList()
                    });

                return Ok(new
                {
                    Environment = new
                    {
                        Environment.MachineName,
                        OsPlatform = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                        Environment.OSVersion,
                        Environment.Is64BitOperatingSystem,
                        framework,
                        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                        Environment.UserInteractive,
                        Environment.CurrentDirectory,
                        Environment.ProcessorCount
                    },
                    packageInstanceDetails,
                    NetworkInterface = SystemInformation.GetNetworkInformation()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to collect system stats");
                return StatusCode(500, e.Message);
            }
        }


        [HttpGet("stats/processes")]
        public async Task<IActionResult> GetSystemProcessStats()
        {
            try
            {
                return Ok((await _processManager.GetNodeProcessesAsync())
                                                .Select(pdm => pdm.ToPackageInstanceDetails()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to collect process stats");
                return StatusCode(500, e.Message);
            }
        }


        [HttpGet("stats/processes/{packageName:required:minlength(1)}")]
        public async Task<IActionResult> GetSystemProcessStatsForPackage(string packageName)
        {

            try
            {
                return Ok((await _processManager.GetNodeProcessesAsync(packageName))
                                                .Select(pdm => pdm.ToPackageInstanceDetails()));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to collect process stats for {packageName}");
                return StatusCode(500, e.Message);
            }
        }


    }
}
