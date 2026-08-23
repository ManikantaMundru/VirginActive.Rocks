using VirginActive.Rocks.Api.Contracts.Responses;
using VirginActive.Rocks.Application.Profiles.Models;
using VirginActive.Rocks.Application.Rocks.Models;

namespace VirginActive.Rocks.Api.Mappings
{
    public static class ResponseMappings
    {
        public static RockResponse ToResponse(this RockDto rock)
        {
            return new RockResponse(
                rock.Id,
                rock.MemberId,
                rock.Title,
                rock.Category,
                rock.DueDate,
                rock.Note,
                rock.Status,
                rock.CreatedAtUtc);
        }

        public static EnrichedProfileResponse ToResponse(this EnrichedMemberProfileResult result)
        {
            var profile = result.Profile is null
                ? null
                : new ProfileResponse(
                    result.Profile.Id,
                    result.Profile.Name,
                    result.Profile.Username,
                    result.Profile.Email,
                    result.Profile.Phone,
                    result.Profile.Website);

            return new EnrichedProfileResponse(
                result.MemberId,
                profile,
                result.Rocks.Select(x => x.ToResponse()).ToArray(),
                new EnrichmentResponse(result.EnrichmentAvailable));
        }
    }
}
