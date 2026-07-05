using System.Globalization;

namespace ProbaMala.Services
{
    // Small helpers for turning the AI's free-text values into typed ones.
    public static class AiParsing
    {
        // Parses a date the AI supplied. Accepts a full ISO/parseable date, or a bare
        // year (e.g. "1864" → 1 Jan 1864). Returns an unspecified-kind DateTime to match
        // the "timestamp without time zone" columns.
        public static bool TryParseFlexibleDate(string? input, out DateTime date)
        {
            date = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            var s = input.Trim();

            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return true;

            if (int.TryParse(s, out var year) && year >= 1800 && year <= DateTime.Today.Year)
            {
                date = new DateTime(year, 1, 1);
                return true;
            }

            return false;
        }
    }
}
