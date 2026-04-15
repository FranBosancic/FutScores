using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class MatchController : Controller
    {
        private readonly MatchMockRepository _matchRepository;

        public MatchController(MatchMockRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public IActionResult Index()
        {
            var matches = _matchRepository.GetAll();
            return View(matches);
        }

        public IActionResult Details(int id)
        {
            var match = _matchRepository.GetById(id);
            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }
    }
}
