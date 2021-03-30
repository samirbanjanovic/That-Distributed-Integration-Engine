using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.Components.Master.Interfaces;
using OnTrac.Integration.Components.Master.Node.AccessService;

namespace OnTrac.Integration.Components.Master.Node.Extensions
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
