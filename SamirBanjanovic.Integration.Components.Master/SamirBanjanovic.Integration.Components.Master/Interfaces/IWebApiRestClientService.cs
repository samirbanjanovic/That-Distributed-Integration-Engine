using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace OnTrac.Integration.Components.Master.Interfaces
{
    public interface IWebApiRestClientService
    {
        IRestClient WebApiClient { get; }
    }
}
