using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.Controllers.Api
{
    [Route("api/players")]
    [ApiController]
    public class PlayerApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PlayerApiController> _logger;

        public PlayerApiController(AppDbContext db, ILogger<PlayerApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlayerDTO>> GetAll(
            [FromQuery] string? q = null,
            [FromQuery] int? clubId = null,
            [FromQuery] int? leagueId = null,
            [FromQuery] Position? position = null)
        {
            var query = _db.Players.AsNoTracking().Include(p => p.Club).AsQueryable();

            if (clubId.HasValue)
                query = query.Where(p => p.ClubId == clubId.Value);

            // Players in a league = players whose club belongs to that league.
            if (leagueId.HasValue)
                query = query.Where(p => p.Club.LeagueId == leagueId.Value);

            if (position.HasValue)
                query = query.Where(p => p.Position == position.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.Nationality.ToLower().Contains(term) ||
                    p.Club.Name.ToLower().Contains(term));
            }

            var result = query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<PlayerDTO> GetById(int id)
        {
            var player = _db.Players.AsNoTracking().Include(p => p.Club).FirstOrDefault(p => p.Id == id);
            return player == null ? NotFound() : Ok(ToDTO(player));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<PlayerDTO> Post([FromBody] PlayerRequest model)
        {
            if (!_db.Clubs.Any(c => c.Id == model.ClubId))
                return BadRequest("Club not found.");

            var entity = new Player
            {
                FirstName   = model.FirstName.Trim(),
                LastName    = model.LastName.Trim(),
                DateOfBirth = model.DateOfBirth,
                Position    = model.Position,
                Nationality = model.Nationality.Trim(),
                ClubId      = model.ClubId!.Value
            };

            _db.Players.Add(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: player {PlayerId} created by {User}.", entity.Id, User.Identity?.Name);
            _db.Entry(entity).Reference(p => p.Club).Load();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDTO(entity));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<PlayerDTO> Put(int id, [FromBody] PlayerRequest model)
        {
            var entity = _db.Players.FirstOrDefault(p => p.Id == id);

            if (entity == null)
                return NotFound();

            if (!_db.Clubs.Any(c => c.Id == model.ClubId))
                return BadRequest("Club not found.");

            entity.FirstName   = model.FirstName.Trim();
            entity.LastName    = model.LastName.Trim();
            entity.DateOfBirth = model.DateOfBirth;
            entity.Position    = model.Position;
            entity.Nationality = model.Nationality.Trim();
            entity.ClubId      = model.ClubId!.Value;

            _db.SaveChanges();
            _db.Entry(entity).Reference(p => p.Club).Load();

            _logger.LogInformation("API: player {PlayerId} updated by {User}.", id, User.Identity?.Name);
            return Ok(ToDTO(entity));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var entity = _db.Players.Include(p => p.Ratings).FirstOrDefault(p => p.Id == id);

            if (entity == null)
                return NotFound();

            if (entity.Ratings.Count > 0)
                return BadRequest("Cannot delete a player that has ratings.");

            _db.Players.Remove(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: player {PlayerId} deleted by {User}.", id, User.Identity?.Name);
            return NoContent();
        }

        private static PlayerDTO ToDTO(Player p) => new()
        {
            Id          = p.Id,
            FirstName   = p.FirstName,
            LastName    = p.LastName,
            DateOfBirth = p.DateOfBirth,
            Position    = p.Position.ToString(),
            Nationality = p.Nationality,
            Club        = new ClubSummaryDTO { Id = p.Club.Id, Name = p.Club.Name }
        };
    }
}
