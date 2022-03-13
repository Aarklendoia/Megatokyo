using Megatokyo.Client.Core;
using Megatokyo.Client.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Megatokyo.Client.Services
{
    internal static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddDadJokesApiClient(
            this IServiceCollection services,
            Action<HttpClient> configureClient) =>
                services.AddHttpClient<IRantsApiClient, RantsApiClient>((httpClient) =>
                {
                    RantsApiClientFactory.ConfigureHttpClientCore(httpClient);
                    configureClient(httpClient);
                });
    }
}
