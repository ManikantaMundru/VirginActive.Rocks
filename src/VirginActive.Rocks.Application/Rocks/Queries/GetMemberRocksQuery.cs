using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Application.Rocks.Queries
{
    public sealed record GetMemberRocksQuery(string MemberId, RockStatus? Status);
}
