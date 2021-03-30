using System;
using System.Collections.Generic;
using LiteDB;

namespace OnTrac.Integration.NodeApi.Data.Entities
{
    public class ProcessDetails
    {
        [BsonId(true)]
        public int ProcessDetailsId { get; set; }

        public string PackageName { get; set; }

        public string Command { get; set; }

        public long SettingsId { get; set; }

        public IDictionary<string, string> Arguments { get; set; }

        public Guid NodeProcessId { get; set; }

        public string ProcessUri { get; set; }

        public DateTime StartDateTime { get; set; }

        public int SystemProcessId { get; set; }
    }
}
