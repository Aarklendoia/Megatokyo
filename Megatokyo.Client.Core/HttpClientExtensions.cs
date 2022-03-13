using System.Net.Http;

namespace Megatokyo.Client.Core
{
    public static class HttpClientExtensions
    {
        public static HttpClient AddRantsHeaders(
                this HttpClient httpClient, string host, string apiKey)
        {
            var headers = httpClient.DefaultRequestHeaders;
            //headers.Add(ApiConstants.HostHeader, new Uri(host).Host);
            //headers.Add(ApiConstants.ApiKeyHeader, apiKey);
            return httpClient;
        }
    }
}
