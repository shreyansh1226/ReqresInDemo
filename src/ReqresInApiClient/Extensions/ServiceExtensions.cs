using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using ReqresInApiClient.Options;
using ReqresInApiClient.Services;
using System;

namespace ReqresInApiClient.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddReqresInClient(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(ReqresInOptions.SectionName);
            var options = section.Get<ReqresInOptions>() ?? new ReqresInOptions();

            services.Configure<ReqresInOptions>(section);
            services.AddMemoryCache();

            services.AddHttpClient<IReqresInService, ReqresInService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
            })
            .AddPolicyHandler(GetRetryPolicy(options));

            return services;
        }


        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ReqresInOptions options)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: options.RetryCount,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}