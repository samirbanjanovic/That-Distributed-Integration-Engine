using System;
using System.IO;
using System.Threading.Tasks;
using OnTrac.Integration.Components.Master.Classes;
using OnTrac.Integration.Components.Master.ComponentHost.Data.Entities;
using OnTrac.Integration.Components.Master.ComponentHost.AccessService;
using OnTrac.Integration.Components.Master.ComponentHost.AccessService.Classes;
using OnTrac.Integration.Components.Master.Interfaces;
using OnTrac.Integration.Components.Master.RestSharp;
using RestSharp;
using static OnTrac.Integration.Components.Master.RestSharp.Extensions;
using System.Collections.Generic;

namespace OnTrac.Integration.Components.Master.ComponentHost.WebApi
{
    public class ComponentHostWebApiAccessService
        : IComponentHostAccessService
        , IWebApiRestClientService
    {
        private static readonly string COMPONENT_HOST_API_ROOT_ROUTE = "/api/componentHost";
        private static readonly string COMPONENT_HOST_API_PACKAGES_ROUTE = $"{COMPONENT_HOST_API_ROOT_ROUTE}/packages";
        private static readonly string COMPONENT_HOST_API_SERVICES_ROUTE = $"{COMPONENT_HOST_API_ROOT_ROUTE}/services";

        public IRestClient WebApiClient { get; }

        public ComponentHostWebApiAccessService(string componentHostInstanceUrl)
        {                     
            WebApiClient = RestClientFactory.GetClient(componentHostInstanceUrl);
            WebApiClient.ConfigureWebRequest(r => r.KeepAlive = false);
        }

        //public ComponentInstanceSettings ComponentInstanceSettings { get; }

        #region component host management

        public async Task<ComponentHostInformation> GetComponentHostInformationAsync()
        {
            return await WebApiClient
                        .SendRequestAsync<ComponentHostInformation>($"{COMPONENT_HOST_API_ROOT_ROUTE}/configuration", Method.GET);                     
        }

        public async Task<ComponentHostInformation> ShutdownComponentHostAsync()
        {
            return await WebApiClient
                        .SendRequestAsync<ComponentHostInformation>($"{COMPONENT_HOST_API_ROOT_ROUTE}/shutdown", Method.POST);
        }

        #endregion component host management

        #region package management

        public async Task<IEnumerable<PackageDetails>> GetComponentHostPackageConfigurationAsync()
        {
            return await WebApiClient
                        .SendRequestAsync<IEnumerable<PackageDetails>>($"{COMPONENT_HOST_API_PACKAGES_ROUTE}/configuration", Method.GET);
        }

        public async Task<PackageDetails> GetComponentHostPackageConfigurationAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync<PackageDetails>($"{COMPONENT_HOST_API_PACKAGES_ROUTE}/{packageName}/configuration", Method.GET);
        }

        public async Task<PackageDetails> UpdateComponentHostPackageAsync(Stream package)
        {
            return await WebApiClient
                        .SendRequestWithFileAsync<PackageDetails>(COMPONENT_HOST_API_PACKAGES_ROUTE, package, isUpdate: true);
        }

        public async Task<PackageDetails> UploadComponentHostPackageAsync(Stream package)
        {
            return await WebApiClient                
                        .SendRequestWithFileAsync<PackageDetails>(COMPONENT_HOST_API_PACKAGES_ROUTE, package);
        }


        #endregion package management

        #region hosted service management


        public async Task<bool> InitializeComponentServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/component/{packageName}/init", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> InitializeMessagePublisherServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/messagePublisher/{packageName}/init", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> StartHostServicesAsync()
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/start", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> StopHostServicesAsync()
        {
            return await WebApiClient
                       .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/stop", Method.PUT)
                       .IsSuccessful();
        }

        public async Task<bool> StartComponentServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/component/{packageName}/start", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> StopComponentServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/component/{packageName}/stop", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> StartMessagePublisherServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/messagePublisher/{packageName}/start", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> StopMessagePublisherServiceAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{COMPONENT_HOST_API_SERVICES_ROUTE}/messagePublisher/{packageName}/start", Method.PUT)
                        .IsSuccessful();
        }

        #endregion hsoted service management
    }
}
