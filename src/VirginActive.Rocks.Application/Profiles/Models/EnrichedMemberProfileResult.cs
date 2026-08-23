using VirginActive.Rocks.Application.Rocks.Models;

namespace VirginActive.Rocks.Application.Profiles.Models
{
    public sealed record EnrichedMemberProfileResult(
     string MemberId,
     MemberProfile? Profile,
     IReadOnlyCollection<RockDto> Rocks,
     bool EnrichmentAvailable);
}
