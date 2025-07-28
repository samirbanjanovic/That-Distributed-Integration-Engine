using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.Core;
using TDIE.ComponentHost.WebApi.Extensions;
using TDIE.PackageManager.Core;

namespace TDIE.ComponentHost.WebApi.Controllers
{
    [Route("api/componentHost")]
    //#if !DEBUG

    //    [Authorize]
    //#endif
    public class ComponentHostController
        : ControllerBase
    {
        private readonly IComponentHostBackgroundService _host;
        private readonly ILogger<ComponentHostController> _logger;
        private readonly Process _process;        

        public ComponentHostController(IComponentHostBackgroundService host, Process process, ILogger<ComponentHostController> logger)
        {
            _host = host;
            _logger = logger;
            _process = process;            
        }

        [HttpGet("configuration")]
        public IActionResult GetConfiguration()
        {
            // returns serialized host with public members showing their current values
            _logger.LogInformation("{Message} {@ObjectProperties}", "Retrieving current host stats", _host);
            return Ok(_host);
        }

        [HttpPost("shutdown")]
        public async Task<IActionResult> ShutDown()
        {
            //_logger.LogInformation("{Message} {@ObjectProperties}", "Shutdown initiated", _host);

            await _host.Shutdown();

            return Ok(_host);
        }
    }
}
