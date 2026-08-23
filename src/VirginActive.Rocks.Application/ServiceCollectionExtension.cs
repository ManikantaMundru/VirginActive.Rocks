using Microsoft.Extensions.DependencyInjection;
using VirginActive.Rocks.Application.Profiles;
using VirginActive.Rocks.Application.Rocks;

namespace VirginActive.Rocks.Application
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IRockService, RockService>();

            services.AddScoped<IGetEnrichedMemberProfileService, GetEnrichedMemberProfileService>();

            return services;
        }
    }
}
