using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.IntegrationTests.Fakes;

namespace VirginActive.Rocks.IntegrationTests
{
    public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string ApiKey = "integration-test-api-key";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                var testSettings = new Dictionary<string, string?>
                {
                    ["Authentication:ApiKey"] = ApiKey,
                    ["ProfileApi:BaseUrl"] = "https://jsonplaceholder.typicode.com/",
                    ["ProfileApi:TimeoutSeconds"] = "5"
                };

                configuration.AddInMemoryCollection(testSettings);
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IProfileClient>();
                services.AddSingleton<IProfileClient, FakeProfileClient>();
            });
        }
    }
}
