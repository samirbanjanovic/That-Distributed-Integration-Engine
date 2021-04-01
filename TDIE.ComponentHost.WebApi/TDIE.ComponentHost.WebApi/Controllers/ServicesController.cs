using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.Core;
using TDIE.ComponentHost.Models;
using TDIE.ComponentHost.WebApi.Extensions;
using TDIE.Core;
using TDIE.PackageManager.Core;
using TDIE.Utilities.Mappers.Core;

namespace TDIE.ComponentHost.WebApi.Controllers
{
    // DISABLE WARNING FOR NOT AWAITING ASYNC CALLS
    // THIS IS DESIRED BEHAVIOR - FIRE AND FORGET 
    // REQUESTS TO THE UNDERLYING HOST
#pragma warning disable CS4014

    [Route("api/componentHost/services")]
    //#if !DEBUG

    //    [Authorize]
    //#endif
    public class ServicesController
        : ControllerBase
    {
        private readonly IComponentHostBackgroundService _host;
        private readonly ILogger<ServicesController> _logger;
        private readonly IObjectMapperService _objectMapperService;
        private readonly IPackageManager _packageManager;

        public ServicesController(IComponentHostBackgroundService host, IObjectMapperService objectMapperService, ILogger<ServicesController> logger, IPackageManager packageManager)
        {
            _host = host;
            _logger = logger;
            _objectMapperService = objectMapperService;
            _packageManager = packageManager;
        }


        // starts all hosted services
        [HttpPut("start")]
        public async Task<IActionResult> Start()
        {
            try
            {
                if (_host.HostState == HostState.AwaitingConfiguration)
                {
                    await _host.ApplyConfigurationAsync();
                }

                IComponentHostServiceResponse serviceRessponse = await _host.StartAsync();

                _logger.LogInformation("{Message} {@ObjectProperties}", "Start hosted services", _host);
                return Ok(serviceRessponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} {@ObjectProperties}", "Failed to start hosted services", _host);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("stop")]
        public async Task<IActionResult> Stop()
        {
            return Ok(await _host.StopAsync());
        }

        // configure the current host and background serivce         
        [HttpPut("component/{packageName:required:minlength(1)}/init")]
        [HttpPut("messagePublisher/{packageName:required:minlength(1)}/init")]
        public async Task<IActionResult> Configure(string packageName, [FromBody] IDictionary<string, string> properties)
        {
            _logger.LogInformation("{Message}", "Submitting configuration");
            if (!ModelState.IsValid)
            {   // configuration did not meet our criteria
                _logger.LogError("{Message} {@ObjectProperties}", $"Invalid configuration for {packageName}", properties);
                return BadRequest(ModelState);
            }

            try
            {
                if (_host.HostState == HostState.AwaitingConfiguration)
                {
                    var configurationModel = await BuildServiceConfigurationModel(packageName, properties);

                    if (HttpContext.HasRoutePart($"/component/{packageName}/init"))
                    {
                        _host.SetComponentConfiguration(configurationModel);
                    }
                    else
                    {
                        _host.SetMessagePublisherConfiguration(configurationModel);
                    }

                    return Ok();
                }
                else
                {
                    return BadRequest($"cannot configure when host is in {_host.HostState} state");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message} {@ObjectProperties}", $"failed to configure package {packageName}", properties);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<ServiceConfigurationModel> BuildServiceConfigurationModel(string packageName, IDictionary<string, string> properties)
        {
            IPackageConfiguration configuration = await _packageManager.GetPackageConfigurationAsync(packageName);

            var packageContentRoot = Path.Combine(_packageManager.PackageRoot, packageName, configuration.ContentRoot);

            var serviceConfigurationModel = _objectMapperService.Map<ServiceConfigurationModel, IPackageConfiguration>(configuration);
            serviceConfigurationModel.Properties = properties.ToDictionary(x => x.Key, x => x.Value);

            serviceConfigurationModel.AssemblyPath = Path.Combine(packageContentRoot, configuration.GetQualifiedAssmeblyPath());

            _logger.LogInformation("{Message} {@ObjectProperties}", "Component configuration loaded", serviceConfigurationModel);

            return serviceConfigurationModel;
        }


        [HttpPut("component/{packageName:required:minlength(1)}/start")]
        public async Task<IActionResult> StartComponent(string packageName)
        {
            IComponentHostServiceResponse serviceResponse = null;

            if (!packageName.IsIgnoreCaseOrdinalEqual(_host.ComponentConfiguration.Name))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Package \"{packageName}\" is not the package loaded into system - expecting \"{ _host.ComponentConfiguration.Name}\"");
            }

            if (_host.HostState == HostState.AwaitingConfiguration)
            {
                serviceResponse = await _host.ApplyConfigurationAsync();
            }

            IComponentHostServiceResponse<IComponentSettings> componentResponse = await _host.StartComponentAsync();

            return Ok((ComponentResponse: componentResponse, ServiceResponse: serviceResponse));
        }

        [HttpPut("component/{packageName:required:minlength(1)}/stop")]
        public async Task<IActionResult> StopComponent(string packageName)
        {
            if (!packageName.IsIgnoreCaseOrdinalEqual(_host.ComponentConfiguration.Name))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Package \"{packageName}\" is not the package loaded into system - expecting \"{ _host.ComponentConfiguration.Name}\"");
            }

            return Ok(await _host.StopComponentAsync());
        }

        [HttpPut("messagePublisher/{packageName:required:minlength(1)}/start")]
        public async Task<ActionResult> StartMessagePublisher(string packageName)
        {
            IComponentHostServiceResponse<IMessagePublisherSettings> messagePublisherResponse = null;
            IComponentHostServiceResponse serviceResponse = null;

            if (!packageName.IsIgnoreCaseOrdinalEqual(_host.ComponentConfiguration.Name))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Package \"{packageName}\" is not the package loaded into system - expecting \"{ _host.ComponentConfiguration.Name}\"");
            }

            if (_host.HostState == HostState.AwaitingConfiguration)
            {
                serviceResponse = await _host.ApplyConfigurationAsync();
            }

            messagePublisherResponse = await _host.StartMessagePublisherAsync();

            return Ok((MessagePublisherResponse: messagePublisherResponse, ServiceResponse: serviceResponse));
        }

        [HttpPut("messagePublisher/{packageName:required:minlength(1)}/stop")]
        public async Task<IActionResult> StopMessagePublisher(string packageName)
        {
            if (!packageName.IsIgnoreCaseOrdinalEqual(_host.ComponentConfiguration.Name))
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Package \"{packageName}\" is not the package loaded into system - expecting \"{ _host.ComponentConfiguration.Name}\"");
            }

            return Ok(await _host.StopMessagePublisherAsync());
        }
    }
}
