using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IHomeRepository
    {
        HomeViewModel GetDashboardViewModel();
    }

    public class HomeRepository : IHomeRepository
    {
        private readonly AppDbContext _dbContext;

        public HomeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Builds all the data shown on the home dashboard.
        // We load all tables upfront into dictionaries so we can do the cross-entity
        // lookups in memory without running a new query per match or per player.
        public HomeViewModel GetDashboardViewModel()
        {
            var leagues  = _dbContext.Leagues.AsNoTracking().ToList();
            var clubs    = _dbContext.Clubs.AsNoTracking().ToList();
            var players  = _dbContext.Players.AsNoTracking().ToList();
            var matches  = _dbContext.Matches.AsNoTracking().ToList();
            var ratings  = _dbContext.Ratings.AsNoTracking().ToList();

            var leagueLookup       = leagues.ToDictionary(l => l.Id);
            var clubLookup         = clubs.ToDictionary(c => c.Id);
            var playerLookup       = players.ToDictionary(p => p.Id);
            var ratingCountByMatch = ratings
                .GroupBy(r => r.MatchId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Most recent 6 matches; first card is "Featured", next two are "Final", rest "Recent"
            var recentMatches = matches
                .OrderByDescending(m => m.Date)
                .Take(6)
                .Select((m, index) =>
                {
                    leagueLookup.TryGetValue(m.LeagueId, out var league);
                    clubLookup.TryGetValue(m.HomeTeamId, out var homeClub);
                    clubLookup.TryGetValue(m.AwayTeamId, out var awayClub);

                    var statusLabel = index == 0 ? "Featured" : index < 3 ? "Final" : "Recent";
                    var statusTone  = index == 0 ? "live"     : index < 3 ? "final" : "recent";

                    return new DashboardMatchCard
                    {
                        MatchId      = m.Id,
                        LeagueName   = league?.Name ?? "Unknown League",
                        HomeTeamId   = m.HomeTeamId,
                        AwayTeamId   = m.AwayTeamId,
                        HomeTeamName = homeClub?.Name ?? "Unknown Home Team",
                        AwayTeamName = awayClub?.Name ?? "Unknown Away Team",
                        HomeGoals    = m.HomeGoals,
                        AwayGoals    = m.AwayGoals,
                        Kickoff      = m.Date,
                        KickoffLabel = m.Date.ToString("MMM dd, yyyy"),
                        StatusLabel  = statusLabel,
                        StatusTone   = statusTone,
                        RatingCount  = ratingCountByMatch.TryGetValue(m.Id, out var rc) ? rc : 0
                    };
                })
                .ToList();

            // Top 4 players by average rating across all their ratings
            var featuredPlayers = ratings
                .GroupBy(r => r.PlayerId)
                .Select(g =>
                {
                    playerLookup.TryGetValue(g.Key, out var player);
                    var club = player != null && clubLookup.TryGetValue(player.ClubId, out var c) ? c : null;

                    return new DashboardFeaturedPlayer
                    {
                        PlayerId      = g.Key,
                        FullName      = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                        ClubName      = club?.Name ?? "Unknown Club",
                        Position      = player?.Position ?? Position.Midfielder,
                        Nationality   = player?.Nationality ?? "Unknown",
                        AverageRating = Math.Round(g.Average(r => (double)r.Score), 1)
                    };
                })
                .OrderByDescending(p => p.AverageRating)
                .ThenBy(p => p.FullName)
                .Take(4)
                .ToList();

            var searchShortcuts = new List<DashboardSearchShortcut>
            {
                new() { Label = "Matches",  Controller = "Match",  Action = "Index", RouteName = "matches-index"  },
                new() { Label = "Players",  Controller = "Player", Action = "Index", RouteName = "players-index"  },
                new() { Label = "Clubs",    Controller = "Club",   Action = "Index", RouteName = "clubs-index"    },
                new() { Label = "Leagues",  Controller = "League", Action = "Index", RouteName = "leagues-index"  }
            };

            // Last 5 results per club (W / D / L) for the form strip
            var clubForms = clubs.Select(club =>
            {
                var clubMatches = matches
                    .Where(m => m.HomeTeamId == club.Id || m.AwayTeamId == club.Id)
                    .OrderByDescending(m => m.Date)
                    .Take(5)
                    .ToList();

                var results = clubMatches.Select(m =>
                {
                    bool isHome      = m.HomeTeamId == club.Id;
                    int  goalsFor    = isHome ? m.HomeGoals : m.AwayGoals;
                    int  goalsAgainst = isHome ? m.AwayGoals : m.HomeGoals;

                    if (goalsFor > goalsAgainst) return "W";
                    if (goalsFor == goalsAgainst) return "D";
                    return "L";
                }).ToList();

                return new ClubFormEntry
                {
                    ClubId   = club.Id,
                    ClubName = club.Name,
                    Results  = results
                };
            }).ToList();

            return new HomeViewModel
            {
                TotalClubs     = clubs.Count,
                TotalPlayers   = players.Count,
                TotalMatches   = matches.Count,
                AverageRating  = ratings.Any() ? Math.Round(ratings.Average(r => (double)r.Score), 1) : 0,
                RecentMatches  = recentMatches,
                FeaturedPlayers = featuredPlayers,
                SearchShortcuts = searchShortcuts,
                ClubForms      = clubForms
            };
        }
    }
}
