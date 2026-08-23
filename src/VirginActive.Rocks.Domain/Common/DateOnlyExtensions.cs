namespace VirginActive.Rocks.Domain.Common
{
    public static class DateOnlyExtensions
    {
        public static (DateOnly Start, DateOnly End) GetQuarterRange(this DateOnly date)
        {
            var quarter = ((date.Month - 1) / 3) + 1;
            var quarterStartMonth = ((quarter - 1) * 3) + 1;

            var start = new DateOnly(date.Year, quarterStartMonth, 1);
            var end = start.AddMonths(3).AddDays(-1);

            return (start, end);
        }
    }
}
