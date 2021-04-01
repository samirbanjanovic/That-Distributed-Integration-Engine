using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.Node.AccessService.Classes
{
    public class NodeBasicProcessInformation 
        : IEquatable<NodeBasicProcessInformation>
    {
        public string MachineName { get; set; }

        public string PackageName { get; set; }

        public string Command { get; set; }

        public IDictionary<string, string> Arguments { get; set; }

        public Guid NodeProcessId { get; set; }

        public string ProcessUri { get; set; }

        public DateTime StartDateTime { get; set; }

        public int SystemProcessId { get; set; }

        public long SettingsId { get; set; }

        public bool Equals(NodeBasicProcessInformation other)
        {
            if(other is null)
            {
                return false;
            }

            return MachineName == other.MachineName
                && PackageName == other.PackageName
                && Command == other.Command
                && NodeProcessId == other.NodeProcessId
                && ProcessUri == other.ProcessUri
                && SystemProcessId == other.SystemProcessId;
        }
    }
}
