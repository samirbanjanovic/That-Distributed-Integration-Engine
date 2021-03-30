 using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Core
{
    public interface IMessage
    {
        string Source { get; set; }
        
        DateTime TimeStamp { get; set; } 

        Guid MessageId { get; }
        
        IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}
