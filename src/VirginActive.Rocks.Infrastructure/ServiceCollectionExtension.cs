using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Infrastructure.Configuration;
using VirginActive.Rocks.Infrastructure.Integrations.JsonPlaceholder;
using VirginActive.Rocks.Infrastructure.Persistence;

namespace VirginActive.Rocks.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // The repository owns the in-memory state, so it must live for the lifetime
            // of the application. A scoped/transient repository would lose Rocks between requests.
            services.AddSingleton<IRockRepository, InMemoryRockRepository>();

            services
                .AddOptions<ProfileApiOptions>()
                .Bind(configuration.GetSection(ProfileApiOptions.SectionName))
                .Validate(x => !string.IsNullOrWhiteSpace(x.BaseUrl), "Profile API BaseUrl is required.")
                .Validate(x => Uri.TryCreate(x.BaseUrl, UriKind.Absolute, out _), "Profile API BaseUrl must be a valid absolute URL.")
                .Validate(x => x.TimeoutSeconds > 0, "Profile API TimeoutSeconds must be greater than zero.")
                .ValidateOnStart();

            services
                .AddHttpClient<IProfileClient, ProfileClient>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<ProfileApiOptions>>().Value;
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                })
                  .AddResilienceHandler("profile-api-resilience", (builder, context) =>
                  {
                      builder.AddRetry(new HttpRetryStrategyOptions
                      {
                          MaxRetryAttempts = 3,
                          Delay = TimeSpan.FromMilliseconds(250),
                          BackoffType = DelayBackoffType.Exponential,
                          UseJitter = true,

                          ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                              .Handle<HttpRequestException>()
                              .HandleResult(response => 
                                  response.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                                  (int)response.StatusCode >= 500),

                          OnRetry = arguments =>
                          {
                              var logger = context.ServiceProvider.GetRequiredService<ILogger<ProfileClient>>();

                              var reason = arguments.Outcome.Exception?.Message
                                  ?? arguments.Outcome.Result?.StatusCode.ToString()
                                  ?? "Unknown";

                              logger.LogWarning(
                                  "Retrying profile API. Attempt {AttemptNumber}, Delay {DelayMilliseconds}ms, Reason {Reason}",
                                  arguments.AttemptNumber + 1,
                                  arguments.RetryDelay.TotalMilliseconds,
                                  reason);

                              return default;
                          }
                      });
                  });

            return services;
        }
    }
}
