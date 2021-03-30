using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OnTrac.Integration.Server.Core
{
    public interface IDataAccessService
    {
        Task RegisterResolvedComponentPackage(IComponentPackageConfiguration packageConfiguration);
    }
}
