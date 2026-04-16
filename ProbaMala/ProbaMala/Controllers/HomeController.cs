using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ClubMockRepository _clubRepository;
        private readonly PlayerMockRepository _playerRepository;
        private readonly MatchMockRepository _matchRepository;
        private readonly RatingMockRepository _ratingRepository;

        public HomeController(
            ILogger<HomeController> logger,
            ClubMockRepository clubRepository,
            PlayerMockRepository playerRepository,
            MatchMockRepository matchRepository,
            RatingMockRepository ratingRepository)
        {
            _logger = logger;
            _clubRepository = clubRepository;
            _playerRepository = playerRepository;
            _matchRepository = matchRepository;
            _ratingRepository = ratingRepository;
        }

        public IActionResult Index()
        {
            var clubs = _clubRepository.GetAll();
            var players = _playerRepository.GetAll();
            var matches = _matchRepository.GetAll();
            var ratings = _ratingRepository.GetAll();

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