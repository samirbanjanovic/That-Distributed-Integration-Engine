using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TDIE.ComponentHost.Core;
using TDIE.Core;

namespace TDIE.ComponentHost.Helpers
{
    internal static class Extensions
    {

        public static bool UsesMessagePublisher(this Type componentType)
        {
            //if the component requires a message publisher we will try and give it 
            //the configured one
            return componentType.GetConstructors()
                                .SelectMany(x => x.GetParameters())
                                .Where(x => x.ParameterType == typeof(IMessagePublisher))
                                .Any();

        }

        public static Task ContinueWithErrorAndLog(this Task task, ILogger logger, string message)
        {
            return task.ContinueWith(t =>
            {
                logger.LogError(t.Exception, "{Message}", message);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static IComponentHostServiceResponse Set(this IComponentHostServiceResponse response, MemberState state, string message)
        {
            response.State = state;
            response.Message = message;

            return response;
        }
    }
}
