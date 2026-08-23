namespace VirginActive.Rocks.Api.Contracts.Responses
{
    public sealed record ProfileResponse(
      int Id,
      string Name,
      string Username,
      string Email,
      string? Phone,
      string? Website);
}
