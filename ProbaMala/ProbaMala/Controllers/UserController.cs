using Microsoft.AspNetCore.Mvc;
using ProbaMala.Data;

namespace ProbaMala.Controllers
{
    [Route("korisnici")]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/users", Name = "users-index")]
        [HttpGet("~/users/list")]
        public IActionResult Index()
        {
            var users = _dbContext.Users
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .ToList();

            return View(users);
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/users/{id:int}", Name = "user-details")]
        [HttpGet("~/users/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
