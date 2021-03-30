using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnTrac.Integration.ComponentHost.WebApi.Extensions
{
    internal static class GeneralExtensions
    {
        public static bool IsIgnoreCaseOrdinalEqual(this string left, string right)
        {
            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool IsValid(this IFormFile package)
        {
            return (package is null || package.Length == 0) == false;
        }
    }
}
