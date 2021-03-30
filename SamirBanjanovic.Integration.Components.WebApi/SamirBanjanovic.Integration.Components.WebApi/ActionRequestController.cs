using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.WebApi
{
    [Route("ActionRequest")]
    public class ActionRequestController
        : Controller
    {
        private readonly IComponent _component;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ActionRequestController> _logger;

        public ActionRequestController(IComponent component, IMessagePublisher messagePublisher, ILogger<ActionRequestController> logger)
        {
            _component = component;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]IDictionary<string, string> properties)
        {
            try
            {
                var requestCorrelation = Guid.NewGuid();

                IMessage webMessage = new WebMessage
                {
                    Source = nameof(ActionRequestController),
                    CorrelationId = requestCorrelation,
                    TimeStamp = DateTime.Now,
                    Properties = BuildMessageProperties(properties)
                };

                using (_logger.BeginScope("{@ObjectProperties} {Correlation} {@ComponentMessage}", webMessage.Properties, webMessage.MessageId, webMessage))
                {
                    try
                    {

                        _logger.LogInformation("{Message}", "Submitting items for publish");

                        await _messagePublisher.PublishAsync(webMessage);
                        return Accepted();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "{Message}", "Error processing POST");
                        return BadRequest();
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, "{Message} {@ObjectProperties}", "POST failed!", properties);
                return BadRequest(ModelState);
            }
            
        }

        private IReadOnlyDictionary<string, string> BuildMessageProperties(IDictionary<string, string> propertiesToAdd)
        {
            var messageProperties = _component.Settings.Properties.ToDictionary(x => x.Key, x => x.Value);
            foreach (var pair in propertiesToAdd)
            {
                messageProperties.Add(pair.Key, pair.Value);
            }

            return messageProperties;
        }
    }
}
