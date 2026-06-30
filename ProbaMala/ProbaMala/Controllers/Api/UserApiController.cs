using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.Controllers.Api
{
    [Route("api/users")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<UserApiController> _logger;

        public UserApiController(AppDbContext db, ILogger<UserApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        private static readonly Expression<Func<User, UserDTO>> ToDTO = u => new UserDTO
        {
            Id        = u.Id,
            FirstName = u.FirstName,
            LastName  = u.LastName,
            Email     = u.Email
        };

        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetAll([FromQuery] string? q = null)
        {
            var query = _db.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term));
            }

            return Ok(query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).Select(ToDTO).ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetById(int id)
        {
            var user = _db.Users.AsNoTracking().Where(u => u.Id == id).Select(ToDTO).FirstOrDefault();
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserDTO> Post([FromBody] UserRequest model)
        {
            if (_db.Users.Any(u => u.Email.ToLower() == model.Email.Trim().ToLower()))
                return BadRequest("A user with that email already exists.");

            var entity = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName  = model.LastName.Trim(),
                Email     = model.Email.Trim()
            };

            _db.Users.Add(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: user {UserId} created by {User}.", entity.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Project(entity.Id));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserDTO> Put(int id, [FromBody] UserRequest model)
        {
            var entity = _db.Users.FirstOrDefault(u => u.Id == id);

            if (entity == null)
                return NotFound();

            if (_db.Users.Any(u => u.Email.ToLower() == model.Email.Trim().ToLower() && u.Id != id))
                return BadRequest("A user with that email already exists.");

            entity.FirstName = model.FirstName.Trim();
            entity.LastName  = model.LastName.Trim();
            entity.Email     = model.Email.Trim();

            _db.SaveChanges();

            _logger.LogInformation("API: user {UserId} updated by {User}.", id, User.Identity?.Name);
            return Ok(Project(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var entity = _db.Users.Include(u => u.Ratings).FirstOrDefault(u => u.Id == id);

            if (entity == null)
                return NotFound();

            if (entity.Ratings.Count > 0)
                return BadRequest("Cannot delete a user that has ratings.");

            _db.Users.Remove(entity);
            _db.SaveChanges();

            _logger.LogInformation("API: user {UserId} deleted by {User}.", id, User.Identity?.Name);
            return NoContent();
        }

        private UserDTO Project(int id) =>
            _db.Users.AsNoTracking().Where(u => u.Id == id).Select(ToDTO).First();
    }
}
