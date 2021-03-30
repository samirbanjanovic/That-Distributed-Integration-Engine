using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Server.Core;

namespace OnTrac.Integration.Server.Classes
{
    public class PlatformConfiguration        
        : IPlatformConfiguration
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public Dictionary<string, string> Properties { get; set; }        
    }
}
