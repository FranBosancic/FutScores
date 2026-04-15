using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class ClubController : Controller
    {
        private readonly ClubMockRepository _clubRepository;
        private readonly LeagueMockRepository _leagueRepository;
        public ClubController(ClubMockRepository clubRepository, LeagueMockRepository leagueRepository)
        {
            _clubRepository = clubRepository;
            _leagueRepository = leagueRepository;
        }

        public IActionResult Index()
        {
            var clubs = _clubRepository.GetAll();

            var clubViewModels = clubs.Select(club =>
            {
                var league = _leagueRepository.GetById(club.LeagueId);

                return new ClubDetailsViewModel
                {
                    Id = club.Id,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = league?.Name ?? "Unknown"
                };
            }).ToList();

            return View(clubViewModels);
        }

        public IActionResult Details(int id)
        {
            var club = _clubRepository.GetById(id);
            if (club == null)
            {
                return NotFound();
            }

            var league = _leagueRepository.GetById(club.LeagueId);
            var viewModel = new ClubDetailsViewModel
            {
                Id = club.Id,
                Name = club.Name,
                FoundedDate = club.FoundedDate,
                LeagueName = league?.Name ?? "Unknown"
            };

            return View(viewModel);
        }
    }
}
