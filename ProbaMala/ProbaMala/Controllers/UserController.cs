using Microsoft.AspNetCore.Mvc;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("korisnici")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/users", Name = "users-index")]
        [HttpGet("~/users/list")]
        public IActionResult Index()
        {
            return View(_userRepository.GetAll());
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/users/{id:int}", Name = "user-details")]
        [HttpGet("~/users/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
