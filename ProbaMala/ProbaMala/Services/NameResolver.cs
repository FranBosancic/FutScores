using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;

namespace ProbaMala.Services
{
    // A resolved club: its id plus the league it belongs to (the league is needed to
    // pre-select the League → Club cascade on the player/rating forms).
    public record ClubRef(int Id, int LeagueId);

    // Central place that turns AI-supplied *names* into database *ids*. This is the
    // authoritative half of AI data entry: the model guesses names, this class decides
    // what actually exists. Each method returns null when it can't confidently match, so
    // the controller can fall back to manual selection.
    public interface INameResolver
    {
        ClubRef? ResolveClub(string? name);
        int? ResolveLeagueId(string? name);
        int? ResolveMatchId(int clubAId, int clubBId);
        int? ResolvePlayerIdInMatch(string? name, int matchId);
    }

    public class NameResolver : INameResolver
    {
        private readonly AppDbContext _db;

        public NameResolver(AppDbContext db)
        {
            _db = db;
        }

        // Matches a club by name: exact (case-insensitive) first, else a single substring
        // match either direction (so "Man City" ↔ "Manchester City" both work).
        public ClubRef? ResolveClub(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var q = name.Trim().ToLower();

            return _db.Clubs
                .AsNoTracking()
                .Where(c => c.Name.ToLower() == q || c.Name.ToLower().Contains(q) || q.Contains(c.Name.ToLower()))
                .OrderBy(c => c.Name.ToLower() == q ? 0 : 1)
                .ThenBy(c => c.Name)
                .Select(c => new ClubRef(c.Id, c.LeagueId))
                .FirstOrDefault();
        }

        public int? ResolveLeagueId(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var q = name.Trim().ToLower();

            return _db.Leagues
                .AsNoTracking()
                .Where(l => l.Name.ToLower() == q || l.Name.ToLower().Contains(q) || q.Contains(l.Name.ToLower()))
                .OrderBy(l => l.Name.ToLower() == q ? 0 : 1)
                .ThenBy(l => l.Name)
                .Select(l => (int?)l.Id)
                .FirstOrDefault();
        }

        // Most recent match between two clubs, in either home/away order (the AI may not
        // know which side was home).
        public int? ResolveMatchId(int clubAId, int clubBId) =>
            _db.Matches
                .AsNoTracking()
                .Where(m => (m.HomeTeamId == clubAId && m.AwayTeamId == clubBId)
                         || (m.HomeTeamId == clubBId && m.AwayTeamId == clubAId))
                .OrderByDescending(m => m.Date)
                .Select(m => (int?)m.Id)
                .FirstOrDefault();

        // Matches a player by name within the two squads of a given match.
        public int? ResolvePlayerIdInMatch(string? name, int matchId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var match = _db.Matches.AsNoTracking().FirstOrDefault(m => m.Id == matchId);
            if (match == null)
                return null;

            var q = name.Trim().ToLower();
            var clubIds = new[] { match.HomeTeamId, match.AwayTeamId };

            return _db.Players
                .AsNoTracking()
                .Where(p => clubIds.Contains(p.ClubId) &&
                    ((p.FirstName + " " + p.LastName).ToLower() == q
                     || (p.FirstName + " " + p.LastName).ToLower().Contains(q)
                     || p.LastName.ToLower() == q))
                .OrderBy(p => (p.FirstName + " " + p.LastName).ToLower() == q ? 0 : 1)
                .ThenBy(p => p.LastName)
                .Select(p => (int?)p.Id)
                .FirstOrDefault();
        }
    }
}
