using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Application.Rocks.Models
{
    public sealed record RockDto(
     Guid Id,
     string MemberId,
     string Title,
     RockCategory Category,
     DateOnly DueDate,
     string? Note,
     RockStatus Status,
     DateTimeOffset CreatedAtUtc);
}
