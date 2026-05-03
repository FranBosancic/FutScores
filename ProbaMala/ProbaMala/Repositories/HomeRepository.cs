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

        public HomeViewModel GetDashboardViewModel()
        {
            var leagues = _dbContext.Leagues.AsNoTracking().ToList();
            var clubs = _dbContext.Clubs.AsNoTracking().ToList();
            var players = _dbContext.Players.AsNoTracking().ToList();
            var matches = _dbContext.Matches.AsNoTracking().ToList();
            var ratings = _dbContext.Ratings.AsNoTracking().ToList();

            var leagueLookup = leagues.ToDictionary(league => league.Id);
            var clubLookup = clubs.ToDictionary(club => club.Id);
            var playerLookup = players.ToDictionary(player => player.Id);

            var recentMatches = matches
                .OrderByDescending(match => match.Date)
                .Take(6)
                .Select((match, index) =>
                {
                    leagueLookup.TryGetValue(match.LeagueId, out var league);
                    clubLookup.TryGetValue(match.HomeTeamId, out var homeClub);
                    clubLookup.TryGetValue(match.AwayTeamId, out var awayClub);

                    var statusLabel = index == 0 ? "Featured" : index < 3 ? "Final" : "Recent";
                    var statusTone = index == 0 ? "live" : index < 3 ? "final" : "recent";

                    return new DashboardMatchCard
                    {
                        MatchId = match.Id,
                        LeagueName = league?.Name ?? "Unknown League",
                        HomeTeamName = homeClub?.Name ?? "Unknown Home Team",
                        AwayTeamName = awayClub?.Name ?? "Unknown Away Team",
                        HomeGoals = match.HomeGoals,
                        AwayGoals = match.AwayGoals,
                        Kickoff = match.Date,
                        KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                        StatusLabel = statusLabel,
                        StatusTone = statusTone
                    };
                })
                .ToList();

            var featuredPlayers = ratings
                .GroupBy(rating => rating.PlayerId)
                .Select(group =>
                {
                    playerLookup.TryGetValue(group.Key, out var player);
                    var club = player != null && clubLookup.TryGetValue(player.ClubId, out var resolvedClub)
                        ? resolvedClub
                        : null;

                    return new DashboardFeaturedPlayer
                    {
                        PlayerId = group.Key,
                        FullName = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                        ClubName = club?.Name ?? "Unknown Club",
                        Position = player?.Position ?? Position.Midfielder,
                        Nationality = player?.Nationality ?? "Unknown",
                        AverageRating = Math.Round(group.Average(item => (double)item.Score), 1)
                    };
                })
                .OrderByDescending(player => player.AverageRating)
                .ThenBy(player => player.FullName)
                .Take(4)
                .ToList();

            var searchShortcuts = new List<DashboardSearchShortcut>
            {
                new() { Label = "Matches", Controller = "Match", Action = "Index", RouteName = "matches-index" },
                new() { Label = "Players", Controller = "Player", Action = "Index", RouteName = "players-index" },
                new() { Label = "Clubs", Controller = "Club", Action = "Index", RouteName = "clubs-index" },
                new() { Label = "Leagues", Controller = "League", Action = "Index", RouteName = "leagues-index" }
            };

            var clubForms = clubs.Select(club =>
            {
                var clubMatches = matches
                    .Where(match => match.HomeTeamId == club.Id || match.AwayTeamId == club.Id)
                    .OrderByDescending(match => match.Date)
                    .Take(5)
                    .ToList();

                var results = clubMatches.Select(match =>
                {
                    bool isHome = match.HomeTeamId == club.Id;
                    int goalsFor = isHome ? match.HomeGoals : match.AwayGoals;
                    int goalsAgainst = isHome ? match.AwayGoals : match.HomeGoals;
                    if (goalsFor > goalsAgainst) return "W";
                    if (goalsFor == goalsAgainst) return "D";
                    return "L";
                }).ToList();

                return new ClubFormEntry
                {
                    ClubName = club.Name,
                    Results = results
                };
            }).ToList();

            return new HomeViewModel
            {
                TotalClubs = clubs.Count,
                TotalPlayers = players.Count,
                TotalMatches = matches.Count,
                AverageRating = ratings.Any() ? Math.Round(ratings.Average(rating => (double)rating.Score), 1) : 0,
                RecentMatches = recentMatches,
                FeaturedPlayers = featuredPlayers,
                SearchShortcuts = searchShortcuts,
                ClubForms = clubForms
            };
        }
    }
}