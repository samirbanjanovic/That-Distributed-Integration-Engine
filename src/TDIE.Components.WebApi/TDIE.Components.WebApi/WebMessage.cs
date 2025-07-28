using System;
using System.Collections.Generic;
using TDIE.Core;

namespace TDIE.Components.WebApi
{
    public class WebMessage
        : IMessage
    {
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid MessageId { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; }
        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}

