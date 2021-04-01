using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace TDIE.Components.Master.RestSharp
{
    public class NewtonsoftJsonRestSerializer
        : IRestSerializer
    {
        public string Serialize(object obj) =>
               JsonConvert.SerializeObject(obj);

        public string Serialize(Parameter bodyParameter) =>
            JsonConvert.SerializeObject(bodyParameter.Value);

        public T Deserialize<T>(IRestResponse response)
        {
            if(response.Content == "[]" || string.IsNullOrEmpty(response.Content))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }
            

        public string[] SupportedContentTypes { get; } =
        {
                "application/json"
                , "text/json"
                , "text/x-json"
                , "text/javascript"
                , "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
