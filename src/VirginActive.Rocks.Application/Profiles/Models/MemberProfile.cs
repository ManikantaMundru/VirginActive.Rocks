namespace VirginActive.Rocks.Application.Profiles.Models
{
    public sealed record MemberProfile(
      int Id,
      string Name,
      string Username,
      string Email,
      string? Phone,
      string? Website);
}
