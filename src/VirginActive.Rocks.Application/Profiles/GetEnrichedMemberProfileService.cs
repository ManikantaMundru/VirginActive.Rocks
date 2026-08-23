using Microsoft.Extensions.Logging;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Application.Exceptions;
using VirginActive.Rocks.Application.Profiles.Models;
using VirginActive.Rocks.Application.Rocks.Mappings;
using VirginActive.Rocks.Domain.Exceptions;

namespace VirginActive.Rocks.Application.Profiles
{
    public sealed class GetEnrichedMemberProfileService(
        IRockRepository rockRepository,
        IProfileClient profileClient,
        ILogger<GetEnrichedMemberProfileService> logger) : IGetEnrichedMemberProfileService
    {
        public async Task<EnrichedMemberProfileResult> ExecuteAsync(string memberId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                throw new RockValidationException(
                    "memberId",
                    "Member ID must not be empty.");
            }

            var normalizedMemberId = memberId.Trim();

            var rocks = await rockRepository.GetByMemberIdAsync(normalizedMemberId, cancellationToken);

            var rockDtos = rocks.Select(rock => rock.ToDto()).ToArray();

            try
            {
                var profile = await profileClient.GetProfileAsync(normalizedMemberId, cancellationToken);

                if (profile is null)
                {
                    logger.LogWarning(
                        "Profile enrichment returned no profile for member {MemberId}",
                        normalizedMemberId);

                    return new EnrichedMemberProfileResult(
                        normalizedMemberId,
                        null,
                        rockDtos,
                        false);
                }

                return new EnrichedMemberProfileResult(
                    normalizedMemberId,
                    profile,
                    rockDtos,
                    true);
            }
            catch (ProfileEnrichmentException exception)
            {
                logger.LogWarning(
                    exception,
                    "Profile enrichment unavailable for member {MemberId}",
                    normalizedMemberId);

                return new EnrichedMemberProfileResult(
                    normalizedMemberId,
                    null,
                    rockDtos,
                    false);
            }
        }
    }
}
