using System.Collections.Concurrent;
using VirginActive.Rocks.Application.Abstractions;
using VirginActive.Rocks.Domain.Entities;

namespace VirginActive.Rocks.Infrastructure.Persistence
{
    public sealed class InMemoryRockRepository : IRockRepository
    {
        private readonly ConcurrentDictionary<Guid, Rock> _rocks = new();

        public Task<Rock> AddAsync(Rock rock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_rocks.TryAdd(rock.Id, rock))
            {
                throw new InvalidOperationException($"Rock '{rock.Id}' already exists.");
            }

            return Task.FromResult(rock);
        }

        public Task<IReadOnlyCollection<Rock>> GetByMemberIdAsync(string memberId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyCollection<Rock> rocks = _rocks.Values
                .Where(x => string.Equals(x.MemberId, memberId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.DueDate)
                .ToArray();

            return Task.FromResult(rocks);
        }

        public Task<Rock?> GetAsync(string memberId, Guid rockId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_rocks.TryGetValue(rockId, out var rock))
            {
                return Task.FromResult<Rock?>(null);
            }

            if (!string.Equals(rock.MemberId, memberId, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<Rock?>(null);
            }

            return Task.FromResult<Rock?>(rock);
        }

        public Task UpdateAsync(Rock rock, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_rocks.ContainsKey(rock.Id))
            {
                throw new InvalidOperationException($"Rock '{rock.Id}' does not exist.");
            }

            _rocks[rock.Id] = rock;

            return Task.CompletedTask;
        }
    }
}
