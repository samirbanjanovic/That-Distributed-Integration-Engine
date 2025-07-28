using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Server.Core
{
    public interface IPlatformConfiguration
    {
        string Name { get; set; }

        Guid Id { get; set; }

        Dictionary<string, string> Properties { get; set; }
    }
}
