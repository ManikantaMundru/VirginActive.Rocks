using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Api.Contracts.Responses
{
    public sealed record RockResponse(
       Guid Id,
       string MemberId,
       string Title,
       RockCategory Category,
       DateOnly DueDate,
       string? Note,
       RockStatus Status,
       DateTimeOffset CreatedAtUtc);
}
