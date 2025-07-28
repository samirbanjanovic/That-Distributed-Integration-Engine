using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Server.Core;

namespace TDIE.Server.Classes
{
    public class ComponentMetadata
        : IComponentMetadata
    {
        public Guid Id { get; set; }
        public string ComponentDirectory { get; set; }
        public IComponentPackageConfiguration ComponentPackageConfiguration { get; set; }        
    }
}
