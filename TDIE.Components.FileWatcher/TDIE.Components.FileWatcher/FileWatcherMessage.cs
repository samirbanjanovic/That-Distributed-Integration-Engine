using System;
using System.Collections.Generic;
using TDIE.Core;

namespace TDIE.Components.FileWatcher
{
    public class FileWatcherMessage
        : IMessage
    {
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid MessageId { get; } = Guid.NewGuid();
        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}
