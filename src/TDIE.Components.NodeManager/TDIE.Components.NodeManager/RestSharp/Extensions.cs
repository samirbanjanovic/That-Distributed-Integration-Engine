using System;
using System.IO;
using System.Threading.Tasks;
using RestSharp;

namespace TDIE.Components.NodeManager.RestSharp
{
    internal static class Extensions
    {
        public static async Task<T> SendRequestAsync<T>(this IRestClient client, string apiRoute, Method httpMethod, Action<IRestRequest> extendRequest = null, bool keepAlive = false)
            where T : class
        {
            client.ConfigureWebRequest(r => r.KeepAlive = keepAlive);
            var request = new RestRequest(apiRoute, httpMethod);

            extendRequest?.Invoke(request);

            var result = await client.ExecuteTaskAsync<T>(request);
            
            return result?.Data;            
        }

        public static async Task<IRestResponse> SendRequestAsync(this IRestClient client, string apiRoute, Method httpMethod, Action<IRestRequest> extendRequest = null, bool keepAlive = false)
        {
            client.ConfigureWebRequest(r => r.KeepAlive = keepAlive);
            var request = new RestRequest(apiRoute, httpMethod);

            extendRequest?.Invoke(request);

            return await client.ExecuteTaskAsync(request).ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task<bool> IsSuccessful(this Task<IRestResponse> response)
        {
            return (await response.ConfigureAwait(false)).IsSuccessful;
        }

        public static async Task<T> SendRequestWithFileAsync<T>(this IRestClient client, string apiRoute, Stream package, bool isUpdate = false)
            where T : class
        {
            Method httpMethod = isUpdate ? Method.PUT : Method.POST;

            using (var fileStream = new MemoryStream())
            {
                await package.CopyToAsync(fileStream)
                             .ConfigureAwait(continueOnCapturedContext: false);


                if (fileStream.TryGetBuffer(out ArraySegment<byte> buffer))
                {
                    return await client.SendRequestAsync<T>(apiRoute, httpMethod, request =>
                            {
                                request.AddFileBytes("package", buffer.Array, "package", contentType: "multipart/form-data");
                            });
                }

                return null;
            }

        }
    }
}
