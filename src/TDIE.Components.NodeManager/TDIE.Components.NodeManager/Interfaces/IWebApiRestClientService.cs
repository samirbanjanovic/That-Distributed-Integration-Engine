using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace TDIE.Components.NodeManager.Interfaces
{
    public interface IWebApiRestClientService
    {
        IRestClient WebApiClient { get; }
    }
}
