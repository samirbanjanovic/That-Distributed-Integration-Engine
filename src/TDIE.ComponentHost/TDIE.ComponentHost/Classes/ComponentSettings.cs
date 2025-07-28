using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Core;

namespace TDIE.ComponentHost.Classes
{
    public class ComponentSettings
        : IComponentSettings
    {
        public string Name { get; internal set; }
        public long Id { get; internal set; }
        public IReadOnlyDictionary<string, string> Properties { get; internal set; }
    }
}
