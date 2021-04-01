using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Server.Core;

namespace TDIE.Server.Classes
{
    public class PlatformConfiguration        
        : IPlatformConfiguration
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public Dictionary<string, string> Properties { get; set; }        
    }
}
