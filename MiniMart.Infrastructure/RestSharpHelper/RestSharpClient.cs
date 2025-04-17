using Newtonsoft.Json;
using RestSharp;

namespace MiniMart.Infrastructure.RestSharpHelper
{
    public class RestSharpClient : IRestSharpClient
    {
        private RestClient CreateClient(string baseUrl)
        {
            var options = new RestClientOptions(baseUrl)
            {
                ThrowOnAnyError = false,
                Timeout = TimeSpan.FromMinutes(5)
            };

            var client = new RestClient(baseUrl);
            return client;
        }

        private RestRequest CreateRequest(string resource, Method method, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(resource, method);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }
            return request;
        }

        public async Task<RestResponse> GetAsync(string baseUrl, string resource, Dictionary<string, string>? headers = null)
        {
            var client = CreateClient(baseUrl);
            var request = CreateRequest(resource, Method.Get, headers);
            return await client.ExecuteAsync(request);
        }

        public async Task<RestResponse> PostAsync(string baseUrl, string resource, object body, Dictionary<string, string>? headers = null)
        {
            body =  JsonConvert.SerializeObject(body);
            var client = CreateClient(baseUrl);
            var request = CreateRequest(resource, Method.Post, headers);
            request.AddJsonBody(body);
            return await client.ExecuteAsync(request);
        }
    }
}
