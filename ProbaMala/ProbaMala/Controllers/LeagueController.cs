using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class LeagueController : Controller
    {
        private readonly LeagueMockRepository _leagueRepository;

        public LeagueController(LeagueMockRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public IActionResult Index()
        {
            var leagues = _leagueRepository.GetAll();
            return View(leagues);
        }

        public IActionResult Details(int id)
        {
            var league = _leagueRepository.GetById(id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }
    }
}
