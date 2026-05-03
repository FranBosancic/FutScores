using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Controllers
{
    [Route("utakmice")]
    public class MatchController : Controller
    {
        private readonly AppDbContext _dbContext;

        public MatchController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/matches", Name = "matches-index")]
        [HttpGet("~/matches/list")]
        public IActionResult Index()
        {
            var matchViewModels = _dbContext.Matches
                .Include(match => match.League)
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .OrderByDescending(match => match.Date)
                .AsEnumerable()
                .Select((match, index) =>
            {
                var statusLabel = index == 0 ? "Featured" : index < 4 ? "Final" : "Recent";
                var statusTone = index == 0 ? "live" : index < 4 ? "final" : "recent";

                return new MatchDetailsViewModel
                {
                    Id = match.Id,
                    LeagueId = match.LeagueId,
                    HomeTeamId = match.HomeTeamId,
                    AwayTeamId = match.AwayTeamId,
                    Date = match.Date,
                    KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                    StatusLabel = statusLabel,
                    StatusTone = statusTone,
                    LeagueName = match.League.Name,
                    HomeTeamName = match.HomeTeam.Name,
                    AwayTeamName = match.AwayTeam.Name,
                    HomeGoals = match.HomeGoals,
                    AwayGoals = match.AwayGoals
                };
            }).ToList();

            return View(matchViewModels);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/matches/{id:int}", Name = "match-details")]
        [HttpGet("~/matches/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _dbContext.Matches
                .Include(match => match.League)
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .Where(match => match.Id == id)
                .AsEnumerable()
                .Select(match => new MatchDetailsViewModel
                {
                    Id = match.Id,
                    LeagueId = match.LeagueId,
                    HomeTeamId = match.HomeTeamId,
                    AwayTeamId = match.AwayTeamId,
                    Date = match.Date,
                    KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                    StatusLabel = "Final",
                    StatusTone = "final",
                    LeagueName = match.League.Name,
                    HomeTeamName = match.HomeTeam.Name,
                    AwayTeamName = match.AwayTeam.Name,
                    HomeGoals = match.HomeGoals,
                    AwayGoals = match.AwayGoals
                })
                .FirstOrDefault();

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
