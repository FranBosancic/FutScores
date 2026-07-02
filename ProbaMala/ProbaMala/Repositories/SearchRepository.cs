using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    // The data tier of global search: fans out across every entity and returns a small,
    // capped slice of matches per type as lightweight SearchResultViewModels. It owns the
    // whole cross-entity search concern in one place, using minimal projections (id +
    // label parts only) rather than the heavy graphs the list repositories load for their
    // pages. See PROJECT-PLAN.md §2.4 for the page tier this merges with.
    public interface ISearchRepository
    {
        // Returns matches across all entities, at most perTypeLimit per entity. A blank
        // query returns nothing — we never dump the whole database into the dropdown.
        List<SearchResultViewModel> Search(string? query, int perTypeLimit = 5);
    }

    public class SearchRepository : ISearchRepository
    {
        private readonly AppDbContext _db;

        // Resolves each entity's details route (by name) into a URL, so links stay in
        // sync with the routing table instead of being hard-coded here.
        private readonly LinkGenerator _links;

        public SearchRepository(AppDbContext db, LinkGenerator links)
        {
            _db = db;
            _links = links;
        }

        public List<SearchResultViewModel> Search(string? query, int perTypeLimit = 5)
        {
            var q = query?.Trim().ToLower() ?? string.Empty;
            if (q.Length == 0)
                return new List<SearchResultViewModel>();

            var results = new List<SearchResultViewModel>();

            // Each block: filter → order → cap → project the few columns we need into
            // memory, then map to the shared result shape (URL built via LinkGenerator,
            // which can't run inside the SQL projection).

            var leagues = _db.Leagues.AsNoTracking()
                .Where(l => l.Name.ToLower().Contains(q))
                .OrderBy(l => l.Name)
                .Take(perTypeLimit)
                .Select(l => new { l.Id, l.Name })
                .ToList();
            results.AddRange(leagues.Select(l => Make("League", l.Name, "league-details", l.Id)));

            var clubs = _db.Clubs.AsNoTracking()
                .Where(c => c.Name.ToLower().Contains(q))
                .OrderBy(c => c.Name)
                .Take(perTypeLimit)
                .Select(c => new { c.Id, c.Name })
                .ToList();
            results.AddRange(clubs.Select(c => Make("Club", c.Name, "club-details", c.Id)));

            var players = _db.Players.AsNoTracking()
                .Where(p => p.FirstName.ToLower().Contains(q)
                         || p.LastName.ToLower().Contains(q)
                         || p.Nationality.ToLower().Contains(q))
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Take(perTypeLimit)
                .Select(p => new { p.Id, p.FirstName, p.LastName })
                .ToList();
            results.AddRange(players.Select(p => Make("Player", $"{p.FirstName} {p.LastName}", "player-details", p.Id)));

            var matches = _db.Matches.AsNoTracking()
                .Where(m => m.HomeTeam.Name.ToLower().Contains(q)
                         || m.AwayTeam.Name.ToLower().Contains(q)
                         || m.League.Name.ToLower().Contains(q))
                .OrderByDescending(m => m.Date)
                .Take(perTypeLimit)
                .Select(m => new { m.Id, Home = m.HomeTeam.Name, Away = m.AwayTeam.Name })
                .ToList();
            results.AddRange(matches.Select(m => Make("Match", $"{m.Home} vs {m.Away}", "match-details", m.Id)));

            var ratings = _db.Ratings.AsNoTracking()
                .Where(r => (r.Comment != null && r.Comment.ToLower().Contains(q))
                         || r.Player.LastName.ToLower().Contains(q)
                         || r.User.LastName.ToLower().Contains(q))
                .OrderByDescending(r => r.Score)
                .Take(perTypeLimit)
                .Select(r => new { r.Id, Player = r.Player.FirstName + " " + r.Player.LastName, r.Score })
                .ToList();
            results.AddRange(ratings.Select(r => Make("Rating", $"{r.Player} · {r.Score}/10", "rating-details", r.Id)));

            var users = _db.Users.AsNoTracking()
                .Where(u => u.FirstName.ToLower().Contains(q)
                         || u.LastName.ToLower().Contains(q)
                         || u.Email.ToLower().Contains(q))
                .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                .Take(perTypeLimit)
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToList();
            results.AddRange(users.Select(u => Make("User", $"{u.FirstName} {u.LastName}", "user-details", u.Id)));

            return results;
        }

        private SearchResultViewModel Make(string category, string title, string routeName, int id) => new()
        {
            Title    = title,
            Category = category,
            Url      = _links.GetPathByName(routeName, new { id }) ?? "/"
        };
    }
}
