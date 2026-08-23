namespace VirginActive.Rocks.Infrastructure.Configuration
{
    public sealed class ProfileApiOptions
    {
        public const string SectionName = "ProfileApi";

        public required string BaseUrl { get; init; }

        public int TimeoutSeconds { get; init; } = 10;
    }
}
