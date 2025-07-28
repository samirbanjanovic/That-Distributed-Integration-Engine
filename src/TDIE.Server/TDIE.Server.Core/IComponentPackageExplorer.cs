using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TDIE.Server.Core
{
    public interface IComponentPackageExplorer
    {
        IEnumerable<FileInfo> GetComponentPackagesToInstall();

        void OnNewPackageAvailable(Func<FileInfo, Task> onPackageAvailableAction);
    }
}
