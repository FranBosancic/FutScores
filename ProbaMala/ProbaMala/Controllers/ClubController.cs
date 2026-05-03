using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Controllers
{
    [Route("klubovi")]
    public class ClubController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ClubController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/clubs", Name = "clubs-index")]
        [HttpGet("~/clubs/list")]
        public IActionResult Index()
        {
            var clubViewModels = _dbContext.Clubs
                .Include(club => club.League)
                .OrderBy(club => club.Name)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name
                })
                .ToList();

            return View(clubViewModels);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/clubs/{id:int}", Name = "club-details")]
        [HttpGet("~/clubs/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _dbContext.Clubs
                .Include(club => club.League)
                .Where(club => club.Id == id)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name
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
