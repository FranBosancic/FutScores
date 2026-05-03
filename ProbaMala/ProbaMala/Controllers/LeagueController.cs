using Microsoft.AspNetCore.Mvc;
using ProbaMala.Data;

namespace ProbaMala.Controllers
{
    [Route("lige")]
    public class LeagueController : Controller
    {
        private readonly AppDbContext _dbContext;

        public LeagueController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/leagues", Name = "leagues-index")]
        [HttpGet("~/leagues/list")]
        public IActionResult Index()
        {
            var leagues = _dbContext.Leagues
                .OrderBy(league => league.Name)
                .ToList();

            return View(leagues);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/leagues/{id:int}", Name = "league-details")]
        [HttpGet("~/leagues/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var league = _dbContext.Leagues.Find(id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }
    }
}
