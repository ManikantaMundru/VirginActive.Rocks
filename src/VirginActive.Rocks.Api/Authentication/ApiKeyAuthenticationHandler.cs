using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace VirginActive.Rocks.Api.Authentication
{
    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration) : base(options, logger, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var suppliedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API key is missing."));
            }

            var configuredApiKey = _configuration["Authentication:ApiKey"];

            if (string.IsNullOrWhiteSpace(configuredApiKey))
            {
                Logger.LogError("API key authentication is not configured.");
                return Task.FromResult(AuthenticateResult.Fail("API key authentication is not configured."));
            }

            if (!KeysMatch(suppliedApiKey.ToString(), configuredApiKey))
            {
                Logger.LogWarning("Invalid API key supplied.");
                return Task.FromResult(AuthenticateResult.Fail("API key is invalid."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "api-key-client"),
                new Claim(ClaimTypes.Name, "API Key Client")
            };

            var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private static bool KeysMatch(string suppliedKey, string configuredKey)
        {
            var suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);
            var configuredBytes = Encoding.UTF8.GetBytes(configuredKey);

            return suppliedBytes.Length == configuredBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(suppliedBytes, configuredBytes);
        }
    }
}
