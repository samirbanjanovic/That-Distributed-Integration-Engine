using System;
using System.Linq;

namespace OnTrac.Integration.ComponentHost.WebApi.Extensions
{
    public static class UriHelpers
    {
        public static bool TrySetUriViaArguments(ref Uri uri, string[] args)
        {
            try
            {
                if (args.Any(x => x.ToLower() == "--url"))
                {
                    string urlArg = args[Array.IndexOf(args, "--url") + 1];

                    Uri.TryCreate(urlArg, UriKind.Absolute, out uri);
                }
            }
            catch
            {
                uri = null;
            }

            return uri != null;
        }
    }
}
