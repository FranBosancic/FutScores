using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;
using System.Security.Claims;

namespace ProbaMala.Controllers.Api
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RatingApiController(AppDbContext db)
        {
            _db = db;
        }

        // Loads player, user and the match (with both clubs) so all three nested
        // summaries can be built in memory.
        private IQueryable<Rating> WithRelations() =>
            _db.Ratings
                .Include(r => r.Player)
                .Include(r => r.User)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam);

        [HttpGet]
        public ActionResult<IEnumerable<RatingDTO>> GetAll(
            [FromQuery] string? q = null,
            [FromQuery] int? playerId = null,
            [FromQuery] int? matchId = null,
            [FromQuery] int? userId = null,
            [FromQuery] int? minScore = null,
            [FromQuery] int? maxScore = null)
        {
            var query = WithRelations().AsNoTracking().AsQueryable();

            if (playerId.HasValue)
                query = query.Where(r => r.PlayerId == playerId.Value);

            if (matchId.HasValue)
                query = query.Where(r => r.MatchId == matchId.Value);

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            if (minScore.HasValue)
                query = query.Where(r => r.Score >= minScore.Value);

            if (maxScore.HasValue)
                query = query.Where(r => r.Score <= maxScore.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                var parsedScore = int.TryParse(q.Trim(), out var score);

                query = query.Where(r =>
                    r.Player.FirstName.ToLower().Contains(term) ||
                    r.Player.LastName.ToLower().Contains(term) ||
                    r.User.FirstName.ToLower().Contains(term) ||
                    r.User.LastName.ToLower().Contains(term) ||
                    r.Match.HomeTeam.Name.ToLower().Contains(term) ||
                    r.Match.AwayTeam.Name.ToLower().Contains(term) ||
                    (r.Comment != null && r.Comment.ToLower().Contains(term)) ||
                    (parsedScore && r.Score == score));
            }

            var result = query
                .OrderByDescending(r => r.Score)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<RatingDTO> GetById(int id)
        {
            var rating = WithRelations().AsNoTracking().FirstOrDefault(r => r.Id == id);
            return rating == null ? NotFound() : Ok(ToDTO(rating));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<RatingDTO> Post([FromBody] RatingRequest model)
        {
            var error = ValidateRefs(model);
            if (error != null)
                return BadRequest(error);

            var entity = new Rating
            {
                PlayerId = model.PlayerId!.Value,
                MatchId  = model.MatchId!.Value,
                UserId   = model.UserId!.Value,
                Score    = model.Score,
                Comment  = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim()
            };

            _db.Ratings.Add(entity);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Project(entity.Id));
        }

        // Admin smije urediti svaku ocjenu; obični korisnik samo vlastitu (vlasništvo
        // se provjerava po Rating.User.AppUserId). Ne-admin zadržava izvornog autora,
        // kao i na webu — ne može prebaciti ocjenu na drugog korisnika.
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<RatingDTO> Put(int id, [FromBody] RatingRequest model)
        {
            var entity = _db.Ratings.Include(r => r.User).FirstOrDefault(r => r.Id == id);

            if (entity == null)
                return NotFound();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (entity.User?.AppUserId != appUserId)
                    return Forbid();

                // Zadrži izvornog autora — ne-admin ne smije mijenjati vlasništvo.
                model.UserId = entity.UserId;
            }

            var error = ValidateRefs(model);
            if (error != null)
                return BadRequest(error);

            entity.PlayerId = model.PlayerId!.Value;
            entity.MatchId  = model.MatchId!.Value;
            entity.UserId   = model.UserId!.Value;
            entity.Score    = model.Score;
            entity.Comment  = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim();

            _db.SaveChanges();

            return Ok(Project(id));
        }

        // Admin briše sve; korisnik smije obrisati samo vlastitu ocjenu (provjera
        // vlasništva: Rating.User.AppUserId mora odgovarati prijavljenom AppUserId).
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var entity = _db.Ratings.Include(r => r.User).FirstOrDefault(r => r.Id == id);

            if (entity == null)
                return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (entity.User?.AppUserId != appUserId)
                    return Forbid();
            }

            _db.Ratings.Remove(entity);
            _db.SaveChanges();

            return NoContent();
        }

        // Returns an error message when a referenced entity is missing, otherwise null.
        private string? ValidateRefs(RatingRequest model)
        {
            if (!_db.Players.Any(p => p.Id == model.PlayerId))
                return "Player not found.";

            if (!_db.Matches.Any(m => m.Id == model.MatchId))
                return "Match not found.";

            if (!_db.Users.Any(u => u.Id == model.UserId))
                return "User not found.";

            return null;
        }

        private RatingDTO Project(int id) =>
            ToDTO(WithRelations().AsNoTracking().First(r => r.Id == id));

        private static RatingDTO ToDTO(Rating r) => new()
        {
            Id      = r.Id,
            Score   = r.Score,
            Comment = r.Comment,
            Player  = new PlayerSummaryDTO
            {
                Id       = r.Player.Id,
                FullName = $"{r.Player.FirstName} {r.Player.LastName}",
                Position = r.Player.Position.ToString()
            },
            Match = new MatchSummaryDTO
            {
                Id           = r.Match.Id,
                Date         = r.Match.Date,
                HomeTeamName = r.Match.HomeTeam.Name,
                AwayTeamName = r.Match.AwayTeam.Name,
                HomeGoals    = r.Match.HomeGoals,
                AwayGoals    = r.Match.AwayGoals
            },
            User = new UserSummaryDTO
            {
                Id       = r.User.Id,
                FullName = $"{r.User.FirstName} {r.User.LastName}"
            }
        };
    }
}
