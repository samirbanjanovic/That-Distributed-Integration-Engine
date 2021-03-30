using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace OnTrac.Integration.Components.Master.Node.AccessService.Classes
{
    public class NodeNetworkInformation
    {
        public string Address { get; set; }

        public AddressFamily AddressFamily { get; set; }

        public bool IsIPv4MappedToIPv6 { get; set; }

        public bool IsIPv6LinkLocal { get; set; }

        public bool IsIPv6Multicast { get; set; }

        public bool IsIPv6SiteLocal { get; set; }

        public bool IsIPv6Teredo { get; set; }

        public long ScopeId { get; set; }

        public long AddressPreferredLifetime { get; set; }

        public long AddressValidLifetime { get; set; }

        public long DhcpLeaseLifetime { get; set; }

        public DuplicateAddressDetectionState DuplicateAddressDetectionState { get; set; }

        public bool IsDnsEligible { get; set; }

        public bool IsTransient { get; set; }

        public int PrefixLength { get; set; }

        public PrefixOrigin PrefixOrigin { get; set; }

        public SuffixOrigin SuffixOrigin { get; set; }
    }
}
