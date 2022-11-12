using Megatokyo.Client.Core.Services;
using System;
using System.Net.Http;

namespace Megatokyo.Client.Core
{
    public static class RantsApiClientFactory
    {
        public static IRantsApiClient Create(string host, string apiKey)
        {
            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            var httpClient = new HttpClient(clientHandler)
            {
                BaseAddress = new Uri(host)
            };

            ConfigureHttpClient(httpClient, host, apiKey);

            return new RantsApiClient(httpClient);
        }

        internal static void ConfigureHttpClient(
                HttpClient httpClient, string host, string apiKey)
        {
            ConfigureHttpClientCore(httpClient);
            httpClient.AddRantsHeaders(host, apiKey);
        }

        public static void ConfigureHttpClientCore(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
        }
    }
}
