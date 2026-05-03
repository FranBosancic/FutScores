using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Controllers
{
    [Route("igraci")]
    public class PlayerController : Controller
    {
        private readonly AppDbContext _dbContext;

        public PlayerController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/players", Name = "players-index")]
        [HttpGet("~/players/list")]
        public IActionResult Index()
        {
            var playerViewModels = _dbContext.Players
                .Include(player => player.Club)
                .OrderBy(player => player.LastName)
                .ThenBy(player => player.FirstName)
                .Select(player => new PlayerDetailsViewModel
                {
                    Id = player.Id,
                    ClubId = player.ClubId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    Nationality = player.Nationality,
                    ClubName = player.Club.Name
                })
                .ToList();

            return View(playerViewModels);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/players/{id:int}", Name = "player-details")]
        [HttpGet("~/players/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _dbContext.Players
                .Include(player => player.Club)
                .Where(player => player.Id == id)
                .Select(player => new PlayerDetailsViewModel
                {
                    Id = player.Id,
                    ClubId = player.ClubId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    Nationality = player.Nationality,
                    ClubName = player.Club.Name
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
