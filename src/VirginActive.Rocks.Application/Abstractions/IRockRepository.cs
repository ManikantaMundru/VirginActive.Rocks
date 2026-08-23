using VirginActive.Rocks.Domain.Entities;

namespace VirginActive.Rocks.Application.Abstractions
{
    public interface IRockRepository
    {
        Task<Rock> AddAsync(Rock rock, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Rock>> GetByMemberIdAsync(string memberId, CancellationToken cancellationToken);

        Task<Rock?> GetAsync(string memberId, Guid rockId, CancellationToken cancellationToken);

        Task UpdateAsync(Rock rock, CancellationToken cancellationToken);
    }
}
