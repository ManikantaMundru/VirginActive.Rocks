using VirginActive.Rocks.Application.Profiles.Models;

namespace VirginActive.Rocks.Application.Profiles
{
    public interface IGetEnrichedMemberProfileService
    {
        Task<EnrichedMemberProfileResult> ExecuteAsync(string memberId, CancellationToken cancellationToken);
    }
}
