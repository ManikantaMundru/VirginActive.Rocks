using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirginActive.Rocks.Api.Contracts.Requests;
using VirginActive.Rocks.Api.Contracts.Responses;
using VirginActive.Rocks.Api.Mappings;
using VirginActive.Rocks.Application.Rocks;
using VirginActive.Rocks.Application.Rocks.Commands;
using VirginActive.Rocks.Application.Rocks.Queries;
using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("members/{memberId}/rocks")]
    public sealed class RocksController(IRockService rockService) : ControllerBase
    {
        private readonly IRockService _rockService = rockService;

        [HttpPost]
        [ProducesResponseType<RockResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RockResponse>> CreateAsync(
            string memberId,
            [FromBody] CreateRockRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateRockCommand(
                memberId,
                request.Title,
                request.Category,
                request.DueDate,
                request.Note);

            var result = await _rockService.CreateAsync(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result.ToResponse());
        }

        [HttpGet]
        [ProducesResponseType<IReadOnlyCollection<RockResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyCollection<RockResponse>>> GetAsync(
            string memberId,
            [FromQuery] RockStatus? status,
            CancellationToken cancellationToken)
        {
            var query = new GetMemberRocksQuery(memberId, status);

            var result = await _rockService.GetByMemberAsync(query, cancellationToken);

            return Ok(result.Select(x => x.ToResponse()).ToArray());
        }

        [HttpPatch("{rockId:guid}")]
        [ProducesResponseType<RockResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<RockResponse>> UpdateStatusAsync(
            string memberId,
            Guid rockId,
            [FromBody] UpdateRockStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateRockStatusCommand(memberId, rockId, request.Status);

            var result = await _rockService.UpdateStatusAsync(command, cancellationToken);

            return Ok(result.ToResponse());
        }
    }
}
