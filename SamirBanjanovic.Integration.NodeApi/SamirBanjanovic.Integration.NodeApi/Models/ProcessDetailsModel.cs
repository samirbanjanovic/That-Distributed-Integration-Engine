using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTrac.Integration.NodeApi.Models
{
    public class ProcessDetailsModel
    {
        public string MachineName { get; } = Environment.MachineName;

        public string PackageName { get; set; }

        public string Command { get; set; }

        public IDictionary<string, string> Arguments { get; set; }

        public Guid NodeProcessId { get; set; }

        public string ProcessUri { get; set; }

        public DateTime StartDateTime { get; set; }

        public int SystemProcessId { get; set; }
    }
}
