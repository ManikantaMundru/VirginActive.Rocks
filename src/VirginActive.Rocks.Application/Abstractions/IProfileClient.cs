using VirginActive.Rocks.Application.Profiles.Models;

namespace VirginActive.Rocks.Application.Abstractions
{
    public interface IProfileClient
    {
        Task<MemberProfile?> GetProfileAsync(
            string memberId,
            CancellationToken cancellationToken);
    }
}
