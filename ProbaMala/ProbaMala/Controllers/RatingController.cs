using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Controllers
{
    [Route("ocjene")]
    public class RatingController : Controller
    {
        private readonly AppDbContext _dbContext;

        public RatingController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/ratings", Name = "ratings-index")]
        [HttpGet("~/ratings/list")]
        public IActionResult Index()
        {
            var ratingViewModels = _dbContext.Ratings
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
                .OrderByDescending(rating => rating.Score)
                .AsEnumerable()
                .Select(rating => new RatingDetailsViewModel
                {
                    Id = rating.Id,
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
                    UserId = rating.UserId,
                    PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                    MatchDescription = $"{rating.Match.HomeTeam.Name} vs {rating.Match.AwayTeam.Name} on {rating.Match.Date:yyyy-MM-dd}",
                    UserName = $"{rating.User.FirstName} {rating.User.LastName}",
                    Score = rating.Score,
                    Comment = rating.Comment
                })
                .ToList();

            return View(ratingViewModels);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/ratings/{id:int}", Name = "rating-details")]
        [HttpGet("~/ratings/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _dbContext.Ratings
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
                .Where(rating => rating.Id == id)
                .AsEnumerable()
                .Select(rating => new RatingDetailsViewModel
                {
                    Id = rating.Id,
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
                    UserId = rating.UserId,
                    PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                    MatchDescription = $"{rating.Match.HomeTeam.Name} vs {rating.Match.AwayTeam.Name} on {rating.Match.Date:yyyy-MM-dd}",
                    UserName = $"{rating.User.FirstName} {rating.User.LastName}",
                    Score = rating.Score,
                    Comment = rating.Comment
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
