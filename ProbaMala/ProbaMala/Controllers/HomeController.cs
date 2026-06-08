using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        // GET /  — the landing dashboard showing recent matches and top-rated players
        [HttpGet("", Name = "home-index")]
        [HttpGet("nadzorna-ploca")]
        [HttpGet("dashboard")]
        public IActionResult Index()
        {
            return View(_homeRepository.GetDashboardViewModel());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("greska")]
        [HttpGet("error", Name = "home-error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
