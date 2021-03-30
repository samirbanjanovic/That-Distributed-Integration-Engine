using System;
using System.Collections.Generic;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.QuartzScheduler
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
