using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

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