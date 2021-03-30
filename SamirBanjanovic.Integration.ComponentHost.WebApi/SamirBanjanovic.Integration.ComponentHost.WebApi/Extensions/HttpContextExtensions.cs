using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnTrac.Integration.ComponentHost.WebApi.Extensions
{
    internal static class HttpContextExtensions
    {
        public static bool HasRoutePart(this HttpContext httpContext, string routePart)
        {
            return httpContext.Request.Path.Value.Contains(routePart, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
