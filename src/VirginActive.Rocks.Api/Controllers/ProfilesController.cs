using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirginActive.Rocks.Api.Contracts.Responses;
using VirginActive.Rocks.Api.Mappings;
using VirginActive.Rocks.Application.Profiles;

namespace VirginActive.Rocks.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("members/{memberId}/profile")]
    public sealed class ProfilesController(IGetEnrichedMemberProfileService profileService) : ControllerBase
    {
        private readonly IGetEnrichedMemberProfileService _profileService = profileService;

        [HttpGet("enriched")]
        [ProducesResponseType<EnrichedProfileResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<EnrichedProfileResponse>> GetEnrichedAsync(string memberId, CancellationToken cancellationToken)
        {
            var result = await _profileService.ExecuteAsync(memberId, cancellationToken);

            return Ok(result.ToResponse());
        }
    }
}
