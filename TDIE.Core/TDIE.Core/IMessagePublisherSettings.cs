using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Core
{
    public interface IMessagePublisherSettings
    {
        string Name { get; }

        long Id { get; }

        IReadOnlyDictionary<string, string> Properties { get; }
    }
}
