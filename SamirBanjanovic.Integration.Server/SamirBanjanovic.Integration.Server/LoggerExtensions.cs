using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using OnTrac.Integration.Server.Core;

namespace OnTrac.Integration.Server
{
    public static class LoggerExtensions
    {
        public static IDisposable ExtendScopeWithPackageConfiguration<T>(this ILogger<T> logger, IComponentPackageConfiguration componentPackageConfiguration)
        {
            return logger.BeginScope("{@ComponentPackage}", componentPackageConfiguration);
        }
    }
}
