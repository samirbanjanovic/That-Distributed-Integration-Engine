using System;
using System.Collections.Generic;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.FileWatcher
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
