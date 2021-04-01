using System.IO;
using System.Threading.Tasks;
using TDIE.Components.Master.Classes;
using TDIE.Components.Master.ComponentHost.Data.Entities;
using TDIE.Components.Master.ComponentHost.AccessService.Classes;
using System.Collections.Generic;

namespace TDIE.Components.Master.ComponentHost.AccessService
{
    public interface IComponentHostAccessService
    {
        //ComponentInstanceSettings ComponentInstanceSettings { get; }

        #region component host management

        Task<ComponentHostInformation> GetComponentHostInformationAsync();

        Task<ComponentHostInformation> ShutdownComponentHostAsync();

        #endregion component host management

        #region package management

        Task<IEnumerable<PackageDetails>> GetComponentHostPackageConfigurationAsync();

        Task<PackageDetails> GetComponentHostPackageConfigurationAsync(string packageName);

        Task<PackageDetails> UploadComponentHostPackageAsync(Stream package);

        Task<PackageDetails> UpdateComponentHostPackageAsync(Stream package);

        #endregion package management

        #region hosted service management

        Task<bool> InitializeComponentServiceAsync( string packageName);
        Task<bool> InitializeMessagePublisherServiceAsync( string packageName);
        Task<bool> StartHostServicesAsync();
        Task<bool> StopHostServicesAsync();
        Task<bool> StartComponentServiceAsync(string packageName);
        Task<bool> StopComponentServiceAsync(string packageName);
        Task<bool> StartMessagePublisherServiceAsync(string packageName);
        Task<bool> StopMessagePublisherServiceAsync(string packageName);

        #endregion hsoted service management

    }
}
