using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.WebApi.Extensions;
using TDIE.PackageManager.Core;
using TDIE.Utilities.Mappers.Core;

namespace TDIE.ComponentHost.WebApi.Controllers
{
    [Route("api/componentHost/packages")]
    public class PackagesController
        : ControllerBase
    {       
        private readonly ILogger<PackagesController> _logger;        
        private readonly IPackageManager _packageManager;

        public PackagesController(IPackageManager packageManager, ILogger<PackagesController> logger)
        {            
            _logger = logger;         
            _packageManager = packageManager;
        }

        // configure the current host and background serivce 
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPackage(IFormFile package)
        {
            _logger.LogInformation("{Message}", "Submitting configuration");
            if (!package.IsValid())
            {   // configuration did not meet our criteria
                _logger.LogError("{Message}", "empty file");
                return BadRequest("no content");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await package.CopyToAsync(stream);

                    IPackageConfiguration packageConfiguration = await _packageManager.ImportPackageAsync(stream);

                    return Ok(packageConfiguration);
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", "Invalid Configuration Model received");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdatePackage(IFormFile package)
        {
            _logger.LogInformation("{Message}", "Submitting configuration");
            if (!package.IsValid())
            {   // configuration did not meet our criteria
                _logger.LogError("{Message}", "empty file");
                return BadRequest("no content");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await package.CopyToAsync(stream);

                    IPackageConfiguration packageConfiguration = await _packageManager.UpdatePackageAsync(stream);

                    return Ok(packageConfiguration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", "Invalid Configuration Model received");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetAllPackageConfigurations()
        {
            return Ok(await _packageManager.GetAllPackageConfigurationsAsync());
        }

        [HttpGet("{packageName:required:minlength(1)}/configuration")]
        public async Task<IActionResult> GetPackageConfiguration(string packageName)
        {
            return Ok(await _packageManager.GetPackageConfigurationAsync(packageName));
        }
    }
}
