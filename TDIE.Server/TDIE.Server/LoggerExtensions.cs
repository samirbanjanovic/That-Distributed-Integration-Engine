using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TDIE.Server.Core;

namespace TDIE.Server
{
    public static class LoggerExtensions
    {
        public static IDisposable ExtendScopeWithPackageConfiguration<T>(this ILogger<T> logger, IComponentPackageConfiguration componentPackageConfiguration)
        {
            return logger.BeginScope("{@ComponentPackage}", componentPackageConfiguration);
        }
    }
}
