using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Core
{
    public interface IPlatformConfiguration
    {
        string Name { get; set; }

        Guid Id { get; set; }

        Dictionary<string, string> Properties { get; set; }
    }
}
