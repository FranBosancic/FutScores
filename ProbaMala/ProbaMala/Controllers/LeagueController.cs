using Microsoft.AspNetCore.Mvc;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("lige")]
    public class LeagueController : Controller
    {
        private readonly ILeagueRepository _leagueRepository;

        public LeagueController(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/leagues", Name = "leagues-index")]
        [HttpGet("~/leagues/list")]
        public IActionResult Index()
        {
            return View(_leagueRepository.GetAll());
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/leagues/{id:int}", Name = "league-details")]
        [HttpGet("~/leagues/details/{id:int}")]
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
