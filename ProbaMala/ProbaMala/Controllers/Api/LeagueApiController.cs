using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.Controllers.Api
{
    [Route("api/leagues")]
    [ApiController]
    public class LeagueApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<LeagueApiController> _logger;

        public LeagueApiController(AppDbContext db, ILogger<LeagueApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Single projection definition reused by every read path so the DTO shape
        // is identical whether it comes from list, detail, or a create/update echo.
        private static readonly Expression<Func<League, LeagueDTO>> ToDTO = l => new LeagueDTO
        {
            Id         = l.Id,
            Name       = l.Name,
            ClubCount  = l.Clubs.Count,
            MatchCount = l.Matches.Count
        };

        [HttpGet]
        public ActionResult<IEnumerable<LeagueDTO>> GetAll([FromQuery] string? q = null)
        {
            var query = _db.Leagues.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(l => l.Name.ToLower().Contains(term));
            }

            return Ok(query.OrderBy(l => l.Name).Select(ToDTO).ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<LeagueDTO> GetById(int id)
        {
            var league = _db.Leagues.AsNoTracking().Where(l => l.Id == id).Select(ToDTO).FirstOrDefault();
            return league == null ? NotFound() : Ok(league);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<LeagueDTO> Post([FromBody] LeagueRequest model)
        {
            if (_db.Leagues.Any(l => l.Name.ToLower() == model.Name.Trim().ToLower()))
                return BadRequest("A league with that name already exists.");

            var entity = new League { Name = model.Name.Trim() };
            _db.Leagues.Add(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: league {LeagueId} created by {User}.", entity.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Project(entity.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<LeagueDTO> Put(int id, [FromBody] LeagueRequest model)
        {
            var entity = _db.Leagues.FirstOrDefault(l => l.Id == id);

            if (entity == null)
                return NotFound();

            if (_db.Leagues.Any(l => l.Name.ToLower() == model.Name.Trim().ToLower() && l.Id != id))
                return BadRequest("A league with that name already exists.");

            entity.Name = model.Name.Trim();
            _db.SaveChanges();

            _logger.LogInformation("API: league {LeagueId} updated by {User}.", id, User.Identity?.Name);
            return Ok(Project(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var entity = _db.Leagues
                .Include(l => l.Clubs)
                .Include(l => l.Matches)
                .FirstOrDefault(l => l.Id == id);

            if (entity == null)
                return NotFound();

            if (entity.Clubs.Count > 0 || entity.Matches.Count > 0)
                return BadRequest("Cannot delete a league that still has clubs or matches.");

            _db.Leagues.Remove(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: league {LeagueId} deleted by {User}.", id, User.Identity?.Name);
            return NoContent();
        }

        // Re-reads the row through the shared projection so create/update responses
        // match the read endpoints exactly.
        private LeagueDTO Project(int id) =>
            _db.Leagues.AsNoTracking().Where(l => l.Id == id).Select(ToDTO).First();
    }
}
