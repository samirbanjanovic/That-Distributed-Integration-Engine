using System.Collections.Generic;
using System.Linq;

namespace TDIE.Components.Master.Classes.Extensions
{
    public enum PackageStateEnum
    {
        MissingPackage = -1,
        UpToDate = 0,
        IncorrectVersion = 1
    }

    public static class PackageDetailsExtensions
    {
        public static PackageStateEnum GetPackageVersionState(this IEnumerable<PackageDetails> packageDetails, string packageName, string packageVersion)
        {
            PackageDetails remotePackage = null;
            if ((remotePackage = packageDetails.FirstOrDefault(x => x.PackageName == packageName)) != null)
            {
                if (remotePackage.PackageVersion == packageVersion)
                {
                    return PackageStateEnum.UpToDate;
                }

                return PackageStateEnum.IncorrectVersion;
            }

            return PackageStateEnum.MissingPackage;
        }
    }
}
