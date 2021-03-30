using RestSharp;

namespace OnTrac.Integration.Components.Master.RestSharp
{
    internal static class RestClientFactory
    {
        public static IRestClient GetClient(string baseUri)
        {
            IRestClient client = new RestClient(baseUri);

            client.UseSerializer(new NewtonsoftJsonRestSerializer())
                  .RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            return client;
        }
    }
}
