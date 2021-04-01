using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDIE.Components.NodeManager.Node.AccessService.Classes;

namespace TDIE.Components.NodeManager.Node.Extensions
{
    public static class NodeResponseClassExtensions
    {
        public static bool HasPackageInstances(this NodeSystemStats nodeSystemStats, string packageName)
        {
            return nodeSystemStats.PackageInstanceDetails
                      .Where(process => process.PackageName == packageName)
                      .Any();
        }
    }
}
