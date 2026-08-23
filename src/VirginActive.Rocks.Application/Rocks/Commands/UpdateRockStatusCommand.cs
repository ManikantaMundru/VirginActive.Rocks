using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Application.Rocks.Commands
{
    public sealed record UpdateRockStatusCommand(string MemberId, Guid RockId, RockStatus Status);
}
