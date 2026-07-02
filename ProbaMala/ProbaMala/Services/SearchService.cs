using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Services
{
    // Global search orchestrator. Owns the static page/menu catalogue and delegates the
    // data tier (leagues, clubs, players, matches, ratings, users) to ISearchRepository,
    // then merges both into one result list. Pages come first (so focusing the box shows
    // the "jump to page" menu), followed by the grouped data matches.
    public interface ISearchService
    {
        // An empty/whitespace query returns the full page catalogue only; a non-empty
        // query returns matching pages plus the data matches from the repository.
        List<SearchResultViewModel> Search(string? query, int pageLimit = 8);
    }

    public class SearchService : ISearchService
    {
        // LinkGenerator resolves the named routes (declared on the controllers) into
        // URLs, so we don't hard-code paths that could drift from the routing table.
        private readonly LinkGenerator _links;
        private readonly ISearchRepository _searchRepository;

        public SearchService(LinkGenerator links, ISearchRepository searchRepository)
        {
            _links = links;
            _searchRepository = searchRepository;
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

        public List<SearchResultViewModel> Search(string? query, int pageLimit = 8)
        {
            var q = query?.Trim().ToLowerInvariant() ?? string.Empty;

            // Matching pages first — the whole catalogue when the query is blank.
            var results = Pages
                .Where(page =>
                    q.Length == 0 ||
                    page.Title.ToLowerInvariant().Contains(q) ||
                    page.Keywords.Contains(q))
                .Take(pageLimit)
                .Select(page => new SearchResultViewModel
                {
                    Title    = page.Title,
                    Category = "Page",
                    Url      = _links.GetPathByName(page.RouteName) ?? "/"
                })
                .ToList();

            // Then the data matches (the repository returns nothing for a blank query).
            results.AddRange(_searchRepository.Search(query));

            return results;
        }
    }
}
