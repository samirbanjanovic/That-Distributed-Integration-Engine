using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OnTrac.Integration.Components.Master.Classes;
using OnTrac.Integration.Components.Master.Interfaces;
using OnTrac.Integration.Components.Master.Node.AccessService;
using OnTrac.Integration.Components.Master.Node.AccessService.Classes;
using OnTrac.Integration.Components.Master.Node.Data.Entities;
using OnTrac.Integration.Components.Master.RestSharp;
using RestSharp;
using static OnTrac.Integration.Components.Master.RestSharp.Extensions;

namespace OnTrac.Integration.Components.Master.Node.WebApi
{
    public class NodeWebApiAccessService
        : INodeAccessService
        , IWebApiRestClientService

    {
        private static readonly string NODE_API_ROOT_ROUTE = "/api/node";
        private static readonly string NODE_API_PACKAGES_ROUTE = $"{NODE_API_ROOT_ROUTE}/packages";
        private static readonly string NODE_API_PROCESSES_ROUTE = $"{NODE_API_ROOT_ROUTE}/processes";

        public IRestClient WebApiClient { get; }

        public NodeWebApiAccessService(NodeServer clientNode)
        {
            ClientNode = clientNode;
            WebApiClient = RestClientFactory.GetClient(ClientNode.NodeApiUri);
            WebApiClient.ConfigureWebRequest(r => r.KeepAlive = false);
        }

        public NodeServer ClientNode { get; }

        #region package management

        public async Task<IEnumerable<PackageDetails>> GetNodePackageConfigurationAsync()
        {            
            return await WebApiClient
                        .SendRequestAsync<IEnumerable<PackageDetails>>($"{NODE_API_PACKAGES_ROUTE}/configuration", Method.GET);         
        }

        public async Task<PackageDetails> GetNodePackageConfigurationAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync<PackageDetails>($"{NODE_API_PACKAGES_ROUTE}/{packageName}/configuration", Method.GET);
        }

        public async Task<PackageDetails> UpdatePackageAsync(Stream package)
        {
            return await WebApiClient
                        .SendRequestWithFileAsync<PackageDetails>(NODE_API_PACKAGES_ROUTE, package, isUpdate: true);
        }

        public async Task<PackageDetails> UploadPackageAsync(Stream package)
        {
            return await WebApiClient
                        .SendRequestWithFileAsync<PackageDetails>(NODE_API_PACKAGES_ROUTE, package);
        }

        #endregion package management


        #region node stats

        public async Task<IEnumerable<NodeProcessInformation>> GetNodeProcessStatsAsync()
        {
            return await WebApiClient
                        .SendRequestAsync<IEnumerable<NodeProcessInformation>>($"{NODE_API_ROOT_ROUTE}/stats/processes", Method.GET) ?? default;
        }

        public async Task<IEnumerable<NodeProcessInformation>> GetNodeProcessStatsAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync<IEnumerable<NodeProcessInformation>>($"{NODE_API_ROOT_ROUTE}/stats/processes/{packageName}", Method.GET);
        }


        public async Task<NodeSystemStats> GetNodeStatsAsync()
        {
            return await WebApiClient
                        .SendRequestAsync<NodeSystemStats>($"{NODE_API_ROOT_ROUTE}/stats", Method.GET);
        }


        #endregion node stats

        #region process management

        public async Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync()
        {
            return await WebApiClient
                            .SendRequestAsync<IEnumerable<NodeBasicProcessInformation>>($"{NODE_API_PROCESSES_ROUTE}", Method.GET);
        }


        public async Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync(string packageName)
        {
            return await WebApiClient
                            .SendRequestAsync<IEnumerable<NodeBasicProcessInformation>>($"{NODE_API_PROCESSES_ROUTE}/{packageName}", Method.GET);
        }

        public async Task<IEnumerable<NodeBasicProcessInformation>> GetPackageProcessInstancesAsync(string packageName, long settingsId)
        {
            return await WebApiClient
                            .SendRequestAsync<IEnumerable<NodeBasicProcessInformation>>($"{NODE_API_PROCESSES_ROUTE}/{packageName}/{settingsId}", Method.GET);
        }

        public async Task<NodeBasicProcessInformation> GetPackageProcessInstanceAsync(string packageName, Guid nodeProcessId)
        {
            return await WebApiClient
                            .SendRequestAsync<NodeBasicProcessInformation>($"{NODE_API_PROCESSES_ROUTE}/{packageName}/{nodeProcessId}", Method.GET);
        }


        public async Task<NodeBasicProcessInformation> StartProcessAsync(string packageName, (long id, IDictionary<string, string> args) processArgs)
        {
            return await WebApiClient
                        .SendRequestAsync<NodeBasicProcessInformation>($"{NODE_API_PROCESSES_ROUTE}/{packageName}/start", Method.PUT, request =>
                        {
                            request.AddJsonBody(processArgs);
                        });
        }

        public async Task<bool> KillProcessAsync(string packageName, Guid nodeProcessId)
        {
            return await WebApiClient
                        .SendRequestAsync($"{NODE_API_PROCESSES_ROUTE}/{packageName}/{nodeProcessId}/kill", Method.PUT)
                        .IsSuccessful();
        }

        public async Task<bool> KillProcessesAsync(string packageName)
        {
            return await WebApiClient
                        .SendRequestAsync($"{NODE_API_PROCESSES_ROUTE}/{packageName}/kill", Method.PUT)
                        .IsSuccessful();
        }


        public async Task<bool> KillProcessesAsync()
        {
            return await WebApiClient
                        .SendRequestAsync($"{NODE_API_PROCESSES_ROUTE}/kill", Method.PUT)
                        .IsSuccessful();
        }

        #endregion process management

    }
}
