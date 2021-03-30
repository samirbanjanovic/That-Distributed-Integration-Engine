using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Core;

namespace OnTrac.Integration.Components.WebApi.Tests
{
    public class ComponentSettings
        : IComponentSettings
    {
        public string Name { get; internal set; }
        public long Id { get; internal set; }
        public IReadOnlyDictionary<string, string> Properties { get; internal set; }
    }
}
