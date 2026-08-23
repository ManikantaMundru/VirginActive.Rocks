using VirginActive.Rocks.Application.Rocks.Commands;
using VirginActive.Rocks.Application.Rocks.Models;
using VirginActive.Rocks.Application.Rocks.Queries;

namespace VirginActive.Rocks.Application.Rocks
{
    public interface IRockService
    {
        Task<RockDto> CreateAsync(CreateRockCommand command, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<RockDto>> GetByMemberAsync(GetMemberRocksQuery query, CancellationToken cancellationToken);

        Task<RockDto> UpdateStatusAsync(UpdateRockStatusCommand command, CancellationToken cancellationToken);
    }
}
