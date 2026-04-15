using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class MatchController : Controller
    {
        private readonly MatchMockRepository _matchRepository;
        private readonly LeagueMockRepository _leagueRepository;
        private readonly ClubMockRepository _clubRepository;

        public MatchController(
            MatchMockRepository matchRepository,
            LeagueMockRepository leagueRepository,
            ClubMockRepository clubRepository)
        {
            _matchRepository = matchRepository;
            _leagueRepository = leagueRepository;
            _clubRepository = clubRepository;
        }

        public IActionResult Index()
        {
            var matches = _matchRepository.GetAll();

            var matchViewModels = matches.Select(match =>
            {
                var league = _leagueRepository.GetById(match.LeagueId);
                var homeClub = _clubRepository.GetById(match.HomeTeamId);
                var awayClub = _clubRepository.GetById(match.AwayTeamId);

                return new MatchDetailsViewModel
                {
                    Id = match.Id,
                    Date = match.Date,
                    LeagueName = league?.Name ?? "Unknown League",
                    HomeTeamName = homeClub?.Name ?? "Unknown Home Team",
                    AwayTeamName = awayClub?.Name ?? "Unknown Away Team",
                    HomeGoals = match.HomeGoals,
                    AwayGoals = match.AwayGoals
                };
            }).ToList();

            return View(matchViewModels);
        }

        public IActionResult Details(int id)
        {
            var match = _matchRepository.GetById(id);
            if (match == null)
            {
                return NotFound();
            }

            var league = _leagueRepository.GetById(match.LeagueId);
            var homeClub = _clubRepository.GetById(match.HomeTeamId);
            var awayClub = _clubRepository.GetById(match.AwayTeamId);

            var viewModel = new MatchDetailsViewModel
            {
                Id = match.Id,
                Date = match.Date,
                LeagueName = league?.Name ?? "Unknown League",
                HomeTeamName = homeClub?.Name ?? "Unknown Home Team",
                AwayTeamName = awayClub?.Name ?? "Unknown Away Team",
                HomeGoals = match.HomeGoals,
                AwayGoals = match.AwayGoals
            };

            return View(viewModel);
        }
    }
}
