using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Application.Rocks.Commands
{
    public sealed record CreateRockCommand(
     string MemberId,
     string Title,
     RockCategory Category,
     DateOnly DueDate,
     string? Note);
}
