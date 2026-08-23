using System.Net;
using System.Net.Http.Json;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Application.Profiles.Models;

namespace VirginActive.Rocks.Infrastructure.Integrations.JsonPlaceholder
{
    public sealed class ProfileClient(HttpClient httpClient) : IProfileClient
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<MemberProfile?> GetProfileAsync(string memberId, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync($"users/{Uri.EscapeDataString(memberId)}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var profile = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken);

            if (profile is null)
            {
                return null;
            }

            return new MemberProfile(
                profile.Id,
                profile.Name,
                profile.Username,
                profile.Email,
                profile.Phone,
                profile.Website);
        }
    }
}
