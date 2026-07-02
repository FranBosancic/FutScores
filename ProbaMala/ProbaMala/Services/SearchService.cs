using ProbaMala.Models.ViewModels;

namespace ProbaMala.Services
{
    // Global search. This first slice covers only the app's navigable pages/menus;
    // the data tier (clubs, players, matches, ratings, users) will be added later and
    // its hits will be merged into the same List<SearchResultViewModel> this returns.
    public interface ISearchService
    {
        // Returns matching results for the query. An empty/whitespace query returns the
        // full page catalogue, so focusing the box acts as a "jump to page" menu.
        List<SearchResultViewModel> Search(string? query, int limit = 8);
    }

    public class SearchService : ISearchService
    {
        // LinkGenerator resolves the named routes (declared on the controllers) into
        // URLs, so we don't hard-code paths that could drift from the routing table.
        private readonly LinkGenerator _links;

        public SearchService(LinkGenerator links)
        {
            _links = links;
        }

        // The searchable page catalogue. Keywords are extra match terms that never show
        // in the UI — e.g. typing "teams" finds Clubs, "fixtures" finds Matches.
        private record PageEntry(string Title, string RouteName, string Keywords);

        private static readonly PageEntry[] Pages =
        {
            new("Dashboard",    "home-index",     "home overview stats start"),
            new("Matches",      "matches-index",  "fixtures results games scores"),
            new("Players",      "players-index",  "squad footballers athletes"),
            new("Clubs",        "clubs-index",    "teams sides"),
            new("Leagues",      "leagues-index",  "competitions divisions"),
            new("Ratings",      "ratings-index",  "reviews performances scores marks"),
            new("Users",        "users-index",    "raters authors accounts people"),
            new("Your ratings", "ratings-mine",   "my mine own submitted"),
        };

        public List<SearchResultViewModel> Search(string? query, int limit = 8)
        {
            var q = query?.Trim().ToLowerInvariant() ?? string.Empty;

            return Pages
                .Where(page =>
                    q.Length == 0 ||
                    page.Title.ToLowerInvariant().Contains(q) ||
                    page.Keywords.Contains(q))
                .Take(limit)
                .Select(page => new SearchResultViewModel
                {
                    Title    = page.Title,
                    Category = "Page",
                    Url      = _links.GetPathByName(page.RouteName) ?? "/"
                })
                .ToList();
        }
    }
}
