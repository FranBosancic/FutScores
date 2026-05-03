using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LeagueMockRepository _leagueRepository;
        private readonly ClubMockRepository _clubRepository;
        private readonly PlayerMockRepository _playerRepository;
        private readonly MatchMockRepository _matchRepository;
        private readonly RatingMockRepository _ratingRepository;

        public HomeController(
            ILogger<HomeController> logger,
            LeagueMockRepository leagueRepository,
            ClubMockRepository clubRepository,
            PlayerMockRepository playerRepository,
            MatchMockRepository matchRepository,
            RatingMockRepository ratingRepository)
        {
            _logger = logger;
            _leagueRepository = leagueRepository;
            _clubRepository = clubRepository;
            _playerRepository = playerRepository;
            _matchRepository = matchRepository;
            _ratingRepository = ratingRepository;
        }

        public IActionResult Index()
        {
            var leagues = _leagueRepository.GetAll();
            var clubs = _clubRepository.GetAll();
            var players = _playerRepository.GetAll();
            var matches = _matchRepository.GetAll();
            var ratings = _ratingRepository.GetAll();

            var recentMatches = matches
                .OrderByDescending(match => match.Date)
                .Take(6)
                .Select((match, index) =>
                {
                    var league = leagues.FirstOrDefault(item => item.Id == match.LeagueId);
                    var homeClub = clubs.FirstOrDefault(item => item.Id == match.HomeTeamId);
                    var awayClub = clubs.FirstOrDefault(item => item.Id == match.AwayTeamId);

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
                    var player = players.FirstOrDefault(item => item.Id == group.Key);
                    var club = player != null ? clubs.FirstOrDefault(item => item.Id == player.ClubId) : null;

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
                new() { Label = "Matches", Controller = "Match", Action = "Index" },
                new() { Label = "Players", Controller = "Player", Action = "Index" },
                new() { Label = "Clubs", Controller = "Club", Action = "Index" },
                new() { Label = "Leagues", Controller = "League", Action = "Index" }
            };

            var clubForms = clubs.Select(club =>
            {
                var clubMatches = matches
                    .Where(m => m.HomeTeamId == club.Id || m.AwayTeamId == club.Id)
                    .OrderByDescending(m => m.Date)
                    .Take(5)
                    .ToList();

                var results = clubMatches.Select(m =>
                {
                    bool isHome = m.HomeTeamId == club.Id;
                    int goalsFor = isHome ? m.HomeGoals : m.AwayGoals;
                    int goalsAgainst = isHome ? m.AwayGoals : m.HomeGoals;
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

            var viewModel = new HomeViewModel
            {
                TotalClubs = clubs.Count(),
                TotalPlayers = players.Count(),
                TotalMatches = matches.Count(),
                AverageRating = ratings.Any() ? Math.Round(ratings.Average(r => (double)r.Score), 1) : 0,
                RecentMatches = recentMatches,
                FeaturedPlayers = featuredPlayers,
                SearchShortcuts = searchShortcuts,
                ClubForms = clubForms
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}