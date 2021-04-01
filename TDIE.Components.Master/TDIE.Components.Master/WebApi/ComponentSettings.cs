using TDIE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDIE.Components.Master.WebApi
{
    public class ComponentSettings
        : IComponentSettings
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public IReadOnlyDictionary<string, string> Properties { get; set; }
    }
}
