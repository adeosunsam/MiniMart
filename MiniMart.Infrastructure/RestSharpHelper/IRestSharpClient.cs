using RestSharp;

namespace MiniMart.Infrastructure.RestSharpHelper
{
    public interface IRestSharpClient
    {
        Task<RestResponse> GetAsync(string baseUrl, string resource, Dictionary<string, string>? headers = null);
        Task<RestResponse> PostAsync(string baseUrl, string resource, object body, Dictionary<string, string>? headers = null);
     }
}
