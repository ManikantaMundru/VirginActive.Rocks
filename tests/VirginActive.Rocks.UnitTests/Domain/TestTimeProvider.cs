namespace VirginActive.Rocks.UnitTests.Domain
{
    public sealed class TestTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = utcNow;

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }
    }
}
