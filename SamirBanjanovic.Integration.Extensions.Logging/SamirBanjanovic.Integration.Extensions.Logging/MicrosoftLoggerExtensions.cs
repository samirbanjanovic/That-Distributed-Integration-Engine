using System;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Extensions.Logging
{
    public static class MicrosoftLoggerExtensions
    {
        public static IDisposable ExtendLogScope<T>(this ILogger<T> logger, IComponent component)
        {
            var resourceType = typeof(T).Name;

            return logger.BeginScope("{@ObjectProperties} {Name} {Correlation} {ResourceType}", component.Settings, component.Name, component.InstanceId, resourceType);
        }
    }
}
