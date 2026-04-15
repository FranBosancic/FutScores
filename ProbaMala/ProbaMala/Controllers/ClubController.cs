using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class ClubController : Controller
    {
        private readonly ClubMockRepository _clubRepository;

        public ClubController(ClubMockRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        public IActionResult Index()
        {
            var clubs = _clubRepository.GetAll();
            return View(clubs);
        }

        public IActionResult Details(int id)
        {
            var club = _clubRepository.GetById(id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }
    }
}
