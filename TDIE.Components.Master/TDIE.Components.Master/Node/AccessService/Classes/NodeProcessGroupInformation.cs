using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.Node.AccessService.Classes
{
    public class NodeProcessGroupInformation
    {
        public string PackageName { get; set; }

        public string Command { get; set; }

        public int Count { get; set; }

        public long TotalWorkingSet64 { get; set; }

        public double AverageWorkingSet64 { get; set; }

        public double TotalProcessorTimeInSeconds { get; set; }

        public double AverageProcessorTimeInSeconds { get; set; }

        public IEnumerable<NodeProcessInformation> Instances { get; set; }
    }
}
