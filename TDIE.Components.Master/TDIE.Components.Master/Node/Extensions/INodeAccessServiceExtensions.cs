using System;
using System.Collections.Generic;
using System.Text;
using TDIE.Components.Master.Interfaces;
using TDIE.Components.Master.Node.AccessService;

namespace TDIE.Components.Master.Node.Extensions
{
    public static class INodeAccessServiceExtensions
    {
        public static INodeAccessService SetKeepAlive(this INodeAccessService nodeAccessService, bool keepAlive)
        {
            var webApiService = nodeAccessService as IWebApiRestClientService;
            webApiService?.WebApiClient.ConfigureWebRequest(rqst => rqst.KeepAlive = keepAlive);

            return nodeAccessService;
        }
    }
}
