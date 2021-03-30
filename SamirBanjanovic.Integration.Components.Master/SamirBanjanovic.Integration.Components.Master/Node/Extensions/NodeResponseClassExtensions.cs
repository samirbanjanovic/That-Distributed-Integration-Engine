using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnTrac.Integration.Components.Master.Node.AccessService.Classes;

namespace OnTrac.Integration.Components.Master.Node.Extensions
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
