using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IMatchRepository
    {
        List<MatchDetailsViewModel> GetAll(string? query = null, int? leagueId = null);
        MatchDetailsViewModel? GetById(int id);
        MatchFormViewModel BuildFormModel();
        MatchFormViewModel? GetFormById(int id);
        void PopulateFormOptions(MatchFormViewModel model);
        List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null);
        bool LeagueExists(int leagueId);
        bool ClubExists(int clubId);
        bool ClubBelongsToLeague(int clubId, int leagueId);
        int Add(MatchFormViewModel model);
        bool Update(int id, MatchFormViewModel model);
        bool Delete(int id);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _dbContext;

        public MatchRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Returns a list of all matches, optionally filtered by text search and/or league.
        // Results are ordered newest first.
        public List<MatchDetailsViewModel> GetAll(string? query = null, int? leagueId = null)
        {
            var matchesQuery = _dbContext.Matches
                .AsNoTracking()
                .Include(m => m.League)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .AsQueryable();

            // Filter to a specific league when coming from the league nav dropdown
            if (leagueId.HasValue)
            {
                matchesQuery = matchesQuery.Where(m => m.LeagueId == leagueId.Value);
            }

            // Text search across team names and league name
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                var parsedGoals = int.TryParse(q, out var goals);

                matchesQuery = matchesQuery.Where(m =>
                    m.League.Name.ToLower().Contains(q) ||
                    m.HomeTeam.Name.ToLower().Contains(q) ||
                    m.AwayTeam.Name.ToLower().Contains(q) ||
                    (parsedGoals && (m.HomeGoals == goals || m.AwayGoals == goals)));
            }

            // We project the rating count inside the SQL query (before .AsEnumerable())
            // because counting a not-included navigation property after materialization
            // always returns 0 when lazy loading is off.
            return matchesQuery
                .OrderByDescending(m => m.Date)
                .Select(m => new { Match = m, RatingCount = m.Ratings.Count })
                .AsEnumerable()
                .Select((row, index) =>
                {
                    // Give the top match a "Featured" badge, the next few "Final",
                    // and the rest just "Recent" — purely cosmetic.
                    var statusLabel = index == 0 ? "Featured" : index < 4 ? "Final" : "Recent";
                    var statusTone  = index == 0 ? "live"     : index < 4 ? "final" : "recent";

                    return new MatchDetailsViewModel
                    {
                        Id            = row.Match.Id,
                        LeagueId      = row.Match.LeagueId,
                        HomeTeamId    = row.Match.HomeTeamId,
                        AwayTeamId    = row.Match.AwayTeamId,
                        Date          = row.Match.Date,
                        KickoffLabel  = row.Match.Date.ToString("MMM dd, yyyy"),
                        StatusLabel   = statusLabel,
                        StatusTone    = statusTone,
                        LeagueName    = row.Match.League.Name,
                        HomeTeamName  = row.Match.HomeTeam.Name,
                        AwayTeamName  = row.Match.AwayTeam.Name,
                        HomeGoals     = row.Match.HomeGoals,
                        AwayGoals     = row.Match.AwayGoals,
                        RatingCount   = row.RatingCount
                    };
                })
                .ToList();
        }

        // Returns the full details for a single match including squads, per-player
        // ratings, and match stats like average rating and player of the match.
        public MatchDetailsViewModel? GetById(int id)
        {
            // Load everything we need in one query to avoid N+1 problems.
            // ThenInclude loads the nested navigation properties (players, users).
            var match = _dbContext.Matches
                .AsNoTracking()
                .Include(m => m.League)
                .Include(m => m.HomeTeam).ThenInclude(club => club.Players)
                .Include(m => m.AwayTeam).ThenInclude(club => club.Players)
                .Include(m => m.Ratings).ThenInclude(r => r.Player)
                .Include(m => m.Ratings).ThenInclude(r => r.User)
                .FirstOrDefault(m => m.Id == id);

            if (match == null)
                return null;

            // Aggregate per-player stats (count and average score) from the ratings list.
            // We do this once and then reuse the dictionary when building the squads.
            var playerStats = match.Ratings
                .GroupBy(r => r.PlayerId)
                .ToDictionary(
                    g => g.Key,
                    g => (Count: g.Count(), Average: g.Average(r => r.Score))
                );

            // Find the player with the highest average rating in this match.
            // If two players tie on average, the one with more ratings wins.
            int? topPlayerId = null;
            if (playerStats.Count > 0)
            {
                topPlayerId = playerStats
                    .OrderByDescending(e => e.Value.Average)
                    .ThenByDescending(e => e.Value.Count)
                    .First().Key;
            }

            // Resolve the top-rated player entity from the already-loaded ratings list
            var topPlayer = topPlayerId.HasValue
                ? match.Ratings.First(r => r.PlayerId == topPlayerId.Value).Player
                : null;

            return new MatchDetailsViewModel
            {
                Id           = match.Id,
                LeagueId     = match.LeagueId,
                HomeTeamId   = match.HomeTeamId,
                AwayTeamId   = match.AwayTeamId,
                Date         = match.Date,
                KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                StatusLabel  = "Final",
                StatusTone   = "final",
                LeagueName   = match.League.Name,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeGoals    = match.HomeGoals,
                AwayGoals    = match.AwayGoals,
                RatingCount  = match.Ratings.Count,

                // Match-level stats
                AverageRating     = ComputeAverage(match.Ratings),
                HomeAverageRating = ComputeAverage(match.Ratings.Where(r => r.Player.ClubId == match.HomeTeamId)),
                AwayAverageRating = ComputeAverage(match.Ratings.Where(r => r.Player.ClubId == match.AwayTeamId)),

                TopRatedPlayerName  = topPlayer != null ? $"{topPlayer.FirstName} {topPlayer.LastName}" : null,
                TopRatedPlayerScore = topPlayerId.HasValue ? playerStats[topPlayerId.Value].Average : null,

                // Build side-by-side squads, each player decorated with their ratings for this match
                HomeSquad = BuildSquad(match.HomeTeam.Players, playerStats, topPlayerId),
                AwaySquad = BuildSquad(match.AwayTeam.Players, playerStats, topPlayerId),

                // Full rating list ordered by score descending, for the ratings panel
                Ratings = match.Ratings
                    .OrderByDescending(r => r.Score)
                    .Select(r => new RatingDetailsViewModel
                    {
                        Id               = r.Id,
                        PlayerId         = r.PlayerId,
                        MatchId          = r.MatchId,
                        UserId           = r.UserId,
                        PlayerName       = $"{r.Player.FirstName} {r.Player.LastName}",
                        MatchDescription = $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} on {match.Date:yyyy-MM-dd}",
                        UserName         = $"{r.User.FirstName} {r.User.LastName}",
                        Score            = r.Score,
                        Comment          = r.Comment,
                        HomeTeamName     = match.HomeTeam.Name,
                        AwayTeamName     = match.AwayTeam.Name,
                        HomeGoals        = match.HomeGoals,
                        AwayGoals        = match.AwayGoals
                    })
                    .ToList()
            };
        }

        public MatchFormViewModel BuildFormModel()
        {
            var model = new MatchFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public MatchFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Matches
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MatchFormViewModel
                {
                    Id         = m.Id,
                    LeagueId   = m.LeagueId,
                    HomeTeamId = m.HomeTeamId,
                    AwayTeamId = m.AwayTeamId,
                    Date       = m.Date,
                    HomeGoals  = m.HomeGoals,
                    AwayGoals  = m.AwayGoals
                })
                .FirstOrDefault();

            if (model == null)
                return null;

            PopulateFormOptions(model);
            return model;
        }

        // Fills the dropdown options on the form model.
        // Dependent dropdowns (home/away team) are only populated when the parent
        // value (league) is already known — e.g. on Edit or after a failed POST.
        public void PopulateFormOptions(MatchFormViewModel model)
        {
            model.LeagueOptions = _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem
                {
                    Value    = l.Id.ToString(),
                    Text     = l.Name,
                    Selected = model.LeagueId == l.Id
                })
                .ToList();

            if (model.LeagueId.HasValue)
            {
                var clubs = GetClubsInLeague(model.LeagueId.Value);
                model.HomeTeamOptions = ToSelectList(clubs, model.HomeTeamId);

                // Away team dropdown excludes the currently selected home team
                var awayClubs = model.HomeTeamId.HasValue
                    ? clubs.Where(c => c.Id != model.HomeTeamId.Value).ToList()
                    : clubs;
                model.AwayTeamOptions = ToSelectList(awayClubs, model.AwayTeamId);
            }
        }

        // Returns clubs in a given league, optionally excluding one club
        // (used by the away-team dropdown to hide the already-selected home team).
        public List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Where(c => c.LeagueId == leagueId && (excludeClubId == null || c.Id != excludeClubId))
                .OrderBy(c => c.Name)
                .Select(c => new CascadeOptionViewModel { Id = c.Id, Label = c.Name })
                .ToList();
        }

        public bool LeagueExists(int leagueId) =>
            _dbContext.Leagues.Any(l => l.Id == leagueId);

        public bool ClubExists(int clubId) =>
            _dbContext.Clubs.Any(c => c.Id == clubId);

        public bool ClubBelongsToLeague(int clubId, int leagueId) =>
            _dbContext.Clubs.Any(c => c.Id == clubId && c.LeagueId == leagueId);

        public int Add(MatchFormViewModel model)
        {
            var entity = new Match
            {
                LeagueId   = model.LeagueId!.Value,
                HomeTeamId = model.HomeTeamId!.Value,
                AwayTeamId = model.AwayTeamId!.Value,
                Date       = model.Date,
                HomeGoals  = model.HomeGoals,
                AwayGoals  = model.AwayGoals
            };

            _dbContext.Matches.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, MatchFormViewModel model)
        {
            var entity = _dbContext.Matches.FirstOrDefault(m => m.Id == id);

            if (entity == null)
                return false;

            entity.LeagueId   = model.LeagueId!.Value;
            entity.HomeTeamId = model.HomeTeamId!.Value;
            entity.AwayTeamId = model.AwayTeamId!.Value;
            entity.Date       = model.Date;
            entity.HomeGoals  = model.HomeGoals;
            entity.AwayGoals  = model.AwayGoals;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Matches.FirstOrDefault(m => m.Id == id);

            if (entity == null)
                return false;

            _dbContext.Matches.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        // ── Private helpers ──────────────────────────────────────────────────

        // Builds the squad view model list for one team, attaching per-player
        // rating stats for this specific match.
        private List<MatchSquadPlayerViewModel> BuildSquad(
            IEnumerable<Player> players,
            Dictionary<int, (int Count, double Average)> stats,
            int? topPlayerId)
        {
            return players
                .Select(p =>
                {
                    stats.TryGetValue(p.Id, out var stat);
                    return new MatchSquadPlayerViewModel
                    {
                        Id           = p.Id,
                        FullName     = $"{p.FirstName} {p.LastName}",
                        Position     = p.Position,
                        RatingCount  = stat.Count,
                        AverageScore = stat.Count > 0 ? stat.Average : (double?)null,
                        IsTopRated   = p.Id == topPlayerId
                    };
                })
                .OrderBy(p => p.Position)
                .ThenBy(p => p.FullName)
                .ToList();
        }

        // Returns the average score for a group of ratings, or null when there are none.
        private static double? ComputeAverage(IEnumerable<Rating> ratings)
        {
            var list = ratings.ToList();
            return list.Count > 0 ? list.Average(r => r.Score) : null;
        }

        // Converts a list of cascade options into a SelectListItem list,
        // pre-marking the currently selected item.
        private static List<SelectListItem> ToSelectList(IEnumerable<CascadeOptionViewModel> options, int? selectedId)
        {
            return options
                .Select(o => new SelectListItem
                {
                    Value    = o.Id.ToString(),
                    Text     = o.Label,
                    Selected = selectedId == o.Id
                })
                .ToList();
        }
    }
}
