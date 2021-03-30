using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.NodeApi.ProcessManagement;
using OnTrac.Integration.PackageManager.Core;

namespace OnTrac.Integration.NodeApi.Controllers
{
    [Route("api/node/packages")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageManager _packageManager;
        private readonly IProcessManager _processManager;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(IPackageManager packageManager, IProcessManager processManager, ILogger<PackagesController> logger)
        {
            _packageManager = packageManager;
            _processManager = processManager;
            _logger = logger;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadPackage(IFormFile package)
        {
            if (package is null || package.Length == 0)
            {   // configuration did not meet our criteria
                _logger.LogError("{Message}", "No content");
                return BadRequest(ModelState);
            }

            try
            {
                // return root of package ex: ~/packages/new-package
                using (var stream = new MemoryStream())
                {
                    await package.CopyToAsync(stream);

                    // change package manager to return extracted pacakge from zip
                    var packageConfiguration = await _packageManager.ImportPackageAsync(stream);

                    return Ok(packageConfiguration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", "Failed to import package");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdatePackage(IFormFile package)
        {
            if (package is null || package.Length == 0)
            {   // configuration did not meet our criteria
                _logger.LogError("{Message}", "No content");
                return BadRequest(ModelState);
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await package.CopyToAsync(stream);
                  
                    // change package manager to return extracted pacakge from zip
                    var packageConfiguration = await _packageManager.UpdatePackageAsync(stream);

                    return Ok(packageConfiguration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to update package");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{packageName:required:minlength(1)}/configuration")]
        public async Task<IActionResult> GetPackageConfiguration(string packageName)
        {
            try
            {                
                return Ok(await _packageManager.GetPackageConfigurationAsync(packageName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to get node packages");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("configuration")]
        public async Task<IActionResult> GetAllPackageConfigurations()
        {
            try
            {
                return Ok(await _packageManager.GetAllPackageConfigurationsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", $"Failed to get node pacakges");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}