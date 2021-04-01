using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TDIE.Server.Core
{
    public interface IComponentPackageInstaller
    {
        Task<IComponentMetadata> TryInstallPackageAsync(FileInfo package);
    }
}
