using System;
using System.Collections.Generic;
using TDIE.Core;

namespace TDIE.Components.QuartzScheduler
{
    public class QuartzMessage
        : IMessage
    {
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid MessageId { get; } = Guid.NewGuid();
        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}
