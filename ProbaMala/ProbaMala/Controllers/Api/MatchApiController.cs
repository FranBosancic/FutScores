using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.Controllers.Api
{
    [Route("api/matches")]
    [ApiController]
    public class MatchApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MatchApiController(AppDbContext db)
        {
            _db = db;
        }

        // Loads the related league + both clubs in one round-trip so the nested
        // summaries can be built in memory.
        private IQueryable<Match> WithRelations() =>
            _db.Matches
                .Include(m => m.League)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam);

        [HttpGet]
        public ActionResult<IEnumerable<MatchDTO>> GetAll([FromQuery] string? q = null, [FromQuery] int? leagueId = null)
        {
            var query = WithRelations().AsNoTracking().AsQueryable();

            if (leagueId.HasValue)
                query = query.Where(m => m.LeagueId == leagueId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(m =>
                    m.League.Name.ToLower().Contains(term) ||
                    m.HomeTeam.Name.ToLower().Contains(term) ||
                    m.AwayTeam.Name.ToLower().Contains(term));
            }

            var result = query
                .OrderByDescending(m => m.Date)
                .ToList()
                .Select(ToDTO)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<MatchDTO> GetById(int id)
        {
            var match = WithRelations().AsNoTracking().FirstOrDefault(m => m.Id == id);
            return match == null ? NotFound() : Ok(ToDTO(match));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<MatchDTO> Post([FromBody] MatchRequest model)
        {
            var error = ValidateTeams(model);
            if (error != null)
                return BadRequest(error);

            var entity = new Match
            {
                LeagueId   = model.LeagueId!.Value,
                HomeTeamId = model.HomeTeamId!.Value,
                AwayTeamId = model.AwayTeamId!.Value,
                Date       = model.Date,
                HomeGoals  = model.HomeGoals,
                AwayGoals  = model.AwayGoals
            };

            _db.Matches.Add(entity);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Project(entity.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<MatchDTO> Put(int id, [FromBody] MatchRequest model)
        {
            var entity = _db.Matches.FirstOrDefault(m => m.Id == id);

            if (entity == null)
                return NotFound();

            var error = ValidateTeams(model);
            if (error != null)
                return BadRequest(error);

            entity.LeagueId   = model.LeagueId!.Value;
            entity.HomeTeamId = model.HomeTeamId!.Value;
            entity.AwayTeamId = model.AwayTeamId!.Value;
            entity.Date       = model.Date;
            entity.HomeGoals  = model.HomeGoals;
            entity.AwayGoals  = model.AwayGoals;

            _db.SaveChanges();

            return Ok(Project(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var entity = _db.Matches.Include(m => m.Ratings).FirstOrDefault(m => m.Id == id);

            if (entity == null)
                return NotFound();

            if (entity.Ratings.Count > 0)
                return BadRequest("Cannot delete a match that has ratings. Delete the ratings first.");

            _db.Matches.Remove(entity);
            _db.SaveChanges();

            return NoContent();
        }

        // Returns an error message when the team selection is invalid, otherwise null.
        private string? ValidateTeams(MatchRequest model)
        {
            if (!_db.Leagues.Any(l => l.Id == model.LeagueId))
                return "League not found.";

            if (model.HomeTeamId == model.AwayTeamId)
                return "Home team and away team must be different.";

            if (!_db.Clubs.Any(c => c.Id == model.HomeTeamId && c.LeagueId == model.LeagueId))
                return "Home team not found in the specified league.";

            if (!_db.Clubs.Any(c => c.Id == model.AwayTeamId && c.LeagueId == model.LeagueId))
                return "Away team not found in the specified league.";

            return null;
        }

        private MatchDTO Project(int id) =>
            ToDTO(WithRelations().AsNoTracking().First(m => m.Id == id));

        private static MatchDTO ToDTO(Match m) => new()
        {
            Id        = m.Id,
            Date      = m.Date,
            HomeGoals = m.HomeGoals,
            AwayGoals = m.AwayGoals,
            League    = new LeagueSummaryDTO { Id = m.League.Id, Name = m.League.Name },
            HomeTeam  = new ClubSummaryDTO { Id = m.HomeTeam.Id, Name = m.HomeTeam.Name },
            AwayTeam  = new ClubSummaryDTO { Id = m.AwayTeam.Id, Name = m.AwayTeam.Name }
        };
    }
}
