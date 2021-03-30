using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Core
{
    public interface IComponentEngine
        : IDisposable
    {
        IEnumerable<IComponentPackageConfiguration> GetComponentPackages();
    }
}
