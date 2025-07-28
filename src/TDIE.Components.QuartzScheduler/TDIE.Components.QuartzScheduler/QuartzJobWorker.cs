using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TDIE.Core;
using Quartz;

namespace TDIE.Components.QuartzScheduler
{
    internal static class QuartzJobWorker
    {
        public static async Task DoWork(JobDataMap jobDataMap)
        {
            var logger = (ILogger)jobDataMap["logger"];

            var properties = (IReadOnlyDictionary<string, string>)jobDataMap["properties"];
            try
            {

                string schedulerName = (string)jobDataMap["name"];
                IMessagePublisher messagePublisher = (IMessagePublisher)jobDataMap["eventPublisher"];

                IMessage quartzMessage = new QuartzMessage
                {
                    Source = schedulerName,
                    TimeStamp = DateTime.Now,
                    Properties = properties
                };

                logger.LogInformation("{Message} {ObjectProperties} {Correlation} {@ComponentMessage}", "Submitting items for publish", quartzMessage.Properties, quartzMessage.MessageId, quartzMessage);

                await messagePublisher.PublishAsync(quartzMessage).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Message} {@ObjectProperties}", "Failed to submit items for publish", properties);
            }
        }
    }
}
