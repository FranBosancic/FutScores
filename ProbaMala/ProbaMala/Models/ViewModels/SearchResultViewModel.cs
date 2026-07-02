namespace ProbaMala.Models.ViewModels
{
    // One row in the global-search dropdown. Kept deliberately generic so the same
    // shape works for every result kind: today it's navigable pages (Category = "Page"),
    // and later the data tier (clubs, players, matches, ratings, users) will produce the
    // same shape with its own Category, all merged into one result list.
    public class SearchResultViewModel
    {
        public string Title { get; set; } = null!;

        // Human-readable group label shown as a badge, e.g. "Page" (later "Club", "Player"…).
        public string Category { get; set; } = null!;

        // Where clicking the result navigates.
        public string Url { get; set; } = null!;
    }
}
