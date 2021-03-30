using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OnTrac.Integration.ComponentHost.WebApi.Extensions
{
    public static class SystemHelpers
    {        
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOs() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static IEnumerable<object> GetNetworkInformation() =>
            NetworkInterface.GetAllNetworkInterfaces()
                            .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                            .Select(a => new
                            {
                                Address = a.Address.ToString(),
                                a.Address.AddressFamily,
                                a.Address.IsIPv4MappedToIPv6,
                                a.Address.IsIPv6LinkLocal,
                                a.Address.IsIPv6Multicast,
                                a.Address.IsIPv6SiteLocal,
                                a.Address.IsIPv6Teredo,
                                ScopeId = a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ? a.Address.ScopeId : -1,
                                a.AddressPreferredLifetime,                                
                                a.AddressValidLifetime,
                                a.DhcpLeaseLifetime,
                                a.DuplicateAddressDetectionState,
                                a.IsDnsEligible,
                                a.IsTransient,
                                a.PrefixLength,
                                a.PrefixOrigin,
                                a.SuffixOrigin
                            });
    }
}
