using Microsoft.Extensions.DependencyInjection;
using VirginActive.Rocks.Domain.Validation.Strategies;

namespace VirginActive.Rocks.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);

            services.AddSingleton<IRockValidationStrategy, RevenueRockValidationStrategy>();

            services.AddSingleton<IRockValidationStrategy, HealthRockValidationStrategy>();

            services.AddSingleton<IRockValidationStrategy, CareerRockValidationStrategy>();

            services.AddSingleton<IRockValidationStrategy, OtherRockValidationStrategy>();

            services.AddSingleton<IRockValidationStrategyResolver, RockValidationStrategyResolver>();

            return services;
        }
    }
}
