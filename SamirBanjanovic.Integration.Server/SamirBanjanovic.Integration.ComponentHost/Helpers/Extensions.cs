using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.ComponentHost._Core;
using OnTrac.Integration.ComponentHost._Core.Interfaces;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.ComponentHost.Helpers
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
