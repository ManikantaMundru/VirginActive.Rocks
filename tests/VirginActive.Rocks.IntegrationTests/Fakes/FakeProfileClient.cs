using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Application.Profiles.Models;

namespace VirginActive.Rocks.IntegrationTests.Fakes
{
    public sealed class FakeProfileClient : IProfileClient
    {
        public Task<MemberProfile?> GetProfileAsync(string memberId, CancellationToken cancellationToken)
        {
            var profile = new MemberProfile(
                1,
                "test member",
                "testuser",
                "test@test.com",
                "123456789",
                "https://example.com");

            return Task.FromResult<MemberProfile?>(profile);
        }
    }
}
