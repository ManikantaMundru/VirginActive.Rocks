using VirginActive.Rocks.Domain.Enums;

namespace VirginActive.Rocks.Api.Contracts.Requests
{
    public sealed record CreateRockRequest(
      string Title,
      RockCategory Category,
      DateOnly DueDate,
      string? Note);
}
