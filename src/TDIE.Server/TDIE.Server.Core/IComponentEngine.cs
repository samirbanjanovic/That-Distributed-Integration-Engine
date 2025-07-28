using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Server.Core
{
    public interface IComponentEngine
        : IDisposable
    {
        IEnumerable<IComponentPackageConfiguration> GetComponentPackages();
    }
}
