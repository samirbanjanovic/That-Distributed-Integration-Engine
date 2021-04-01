using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.Classes;
using TDIE.ComponentHost.Models;
using TDIE.Core;
using OnTrac.Utilities.Mappers.Core;

namespace TDIE.ComponentHost.Controllers
{
    // DISABLE WARNING FOR NOT AWAITING ASYNC CALLS
    // THIS IS DESIRED BEHAVIOR - FIRE AND FORGET 
    // REQUESTS TO THE UNDERLYING HOST
    #pragma warning disable CS4014  
    [Route("Services")]
    public class ServicesController
        : Controller
    {
        private readonly ComponentHostService _host;
        private readonly ILogger<ServicesController> _logger;
        private readonly IObjectMapperService _objectMapperService;

        public ServicesController(ComponentHostService host, IObjectMapperService objectMapperService, ILogger<ServicesController> logger)
        {
            _host = host;
            _logger = logger;
            _objectMapperService = objectMapperService;
        }

        [HttpGet]
        public IActionResult GetHostDetails()
        {
            _logger.LogInformation("{Message} {@ObjectProperties}", "Serializaing HOST", _host);
            return Ok(_host);
        }

        [HttpPost("configure")]
        public IActionResult Configure([FromBody]ServiceConfiguration componentConfiguration)
        {
            _logger.LogInformation("{Message}", "Submitting configuration");
            if (!ModelState.IsValid)
            {
                _logger.LogError("{Message} {@ObjectProperties}", "Invalid Configuration Model received", componentConfiguration);
                return BadRequest(ModelState);
            }

            try
            {
                var serviceConfigurationModel = _objectMapperService.Map<ServiceConfigurationModel, ServiceConfiguration>(componentConfiguration);
                serviceConfigurationModel.MessagePublisherConfiguration = new MessagePublisherConfigurationModel();

                _objectMapperService.Map(serviceConfigurationModel.MessagePublisherConfiguration, componentConfiguration.MessagePublisherConfiguration);
                _host.ConfigureAsync(serviceConfigurationModel);

                return Accepted();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Message} {@ObjectProperties}", "Invalid Configuration Model received", componentConfiguration);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("start")]
        public IActionResult Start()
        {
            try
            {
                _host.StartAsync();

                _logger.LogInformation("{Message} {@ObjectProperties}", "Start hosted members", _host);
                return Accepted();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "{Message} {@ObjectProperties}", "Failed to start hosted members", _host);
                return BadRequest(ModelState);
            }
            
        }

        [HttpPut("stop")]
        public IActionResult Stop()
        {
            _host.StopAsync();
                           
            return Accepted();
        }


        [HttpPut("component/start")]              
        public IActionResult StartComponent()
        {
            _host.StartComponentAsync();

            return Accepted();
        }

        [HttpPut("component/stop")]        
        public IActionResult StopComponent()
        {
            _host.StopComponentAsync();
            return Accepted();
        }

        [HttpPut("messagePublisher/start")]
        public ActionResult StartMessagePublisher()
        {
            _host.StartMessagePublisherAsync();
            return Accepted();
        }

        [HttpPut("messagePublisher/stop")]
        public IActionResult StopMessagePublisher()
        {
            _host.StopMessagePublisherAsync();

            return Accepted();
        }
    }
}
