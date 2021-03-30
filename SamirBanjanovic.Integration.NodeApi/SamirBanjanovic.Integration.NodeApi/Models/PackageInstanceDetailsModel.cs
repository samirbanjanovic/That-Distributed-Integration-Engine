using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTrac.Integration.NodeApi.Models
{
    public class PackageInstanceDetailsModel
    {
        public string MachineName { get; } = Environment.MachineName;

        public string PackageName { get; set; }

        public string Command { get; set; }

        public IDictionary<string, string> Arguments { get; set; }

        public string ProcessUri { get; set; }

        public Guid NodeProcessId { get; set; }

        public DateTime StartDateTime { get; set; }

        public int SystemProcessId { get; set; }

        public string SystemProcessName { get; set; }

        public long WorkingSet64 { get; set; }

        public long MinWorkingSet { get; set; }

        public long MaxWorkingSet { get; set; }

        public double ProcessorTimeInSeconds { get; set; }

        public int ThreadCount { get; set; }

        public long ProcessorAffinity { get; set; }

        public string ModuleName { get; set; }

        public string FileName { get; set; }

        public int ModuleMemorySize { get; set; }
    }
}
