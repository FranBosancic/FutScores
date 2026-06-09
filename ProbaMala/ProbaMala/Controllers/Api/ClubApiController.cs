using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.Controllers.Api
{
    [Route("api/clubs")]
    [ApiController]
    public class ClubApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClubApiController(AppDbContext db)
        {
            _db = db;
        }

        // Nested LeagueSummaryDTO + counts; translates cleanly to a single SQL query.
        private static readonly Expression<Func<Club, ClubDTO>> ToDTO = c => new ClubDTO
        {
            Id          = c.Id,
            Name        = c.Name,
            FoundedDate = c.FoundedDate,
            League      = new LeagueSummaryDTO { Id = c.League.Id, Name = c.League.Name },
            PlayerCount = c.Players.Count,
            MatchCount  = c.HomeMatches.Count + c.AwayMatches.Count
        };

        [HttpGet]
        public ActionResult<IEnumerable<ClubDTO>> GetAll([FromQuery] string? q = null, [FromQuery] int? leagueId = null)
        {
            var query = _db.Clubs.AsNoTracking().AsQueryable();

            if (leagueId.HasValue)
                query = query.Where(c => c.LeagueId == leagueId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    c.League.Name.ToLower().Contains(term));
            }

            return Ok(query.OrderBy(c => c.Name).Select(ToDTO).ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<ClubDTO> GetById(int id)
        {
            var club = _db.Clubs.AsNoTracking().Where(c => c.Id == id).Select(ToDTO).FirstOrDefault();
            return club == null ? NotFound() : Ok(club);
        }

        [HttpPost]
        public ActionResult<ClubDTO> Post([FromBody] ClubRequest model)
        {
            if (!_db.Leagues.Any(l => l.Id == model.LeagueId))
                return BadRequest("League not found.");

            if (_db.Clubs.Any(c => c.Name.ToLower() == model.Name.Trim().ToLower()))
                return BadRequest("A club with that name already exists.");

            var entity = new Club
            {
                Name        = model.Name.Trim(),
                FoundedDate = model.FoundedDate,
                LeagueId    = model.LeagueId!.Value
            };

            _db.Clubs.Add(entity);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Project(entity.Id));
        }

        [HttpPut("{id}")]
        public ActionResult<ClubDTO> Put(int id, [FromBody] ClubRequest model)
        {
            var entity = _db.Clubs.FirstOrDefault(c => c.Id == id);

            if (entity == null)
                return NotFound();

            if (!_db.Leagues.Any(l => l.Id == model.LeagueId))
                return BadRequest("League not found.");

            if (_db.Clubs.Any(c => c.Name.ToLower() == model.Name.Trim().ToLower() && c.Id != id))
                return BadRequest("A club with that name already exists.");

            entity.Name        = model.Name.Trim();
            entity.FoundedDate = model.FoundedDate;
            entity.LeagueId    = model.LeagueId!.Value;

            _db.SaveChanges();

            return Ok(Project(id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _db.Clubs
                .Include(c => c.Players)
                .Include(c => c.HomeMatches)
                .Include(c => c.AwayMatches)
                .FirstOrDefault(c => c.Id == id);

            if (entity == null)
                return NotFound();

            if (entity.Players.Count > 0 || entity.HomeMatches.Count > 0 || entity.AwayMatches.Count > 0)
                return BadRequest("Cannot delete a club that has players or matches.");

            _db.Clubs.Remove(entity);
            _db.SaveChanges();

            return NoContent();
        }

        private ClubDTO Project(int id) =>
            _db.Clubs.AsNoTracking().Where(c => c.Id == id).Select(ToDTO).First();
    }
}
