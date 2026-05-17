using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
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
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_userRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/users/filter", Name = "users-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_UserList", _userRepository.GetAll(q));
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

        [HttpGet("novo")]
        [HttpGet("~/users/create", Name = "user-create")]
        public IActionResult Create()
        {
            return View(_userRepository.BuildFormModel());
        }

        [HttpPost("novo")]
        [HttpPost("~/users/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFormViewModel model)
        {
            ValidateUserForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/users/edit/{id:int}", Name = "user-edit")]
        public IActionResult Edit(int id)
        {
            var model = _userRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/users/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidateUserForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = _userRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/users/delete/{id:int}", Name = "user-delete")]
        public IActionResult Delete(int id)
        {
            var model = _userRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/users/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _userRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateUserForm(UserFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && _userRepository.EmailExists(model.Email, model.Id == 0 ? null : model.Id))
            {
                ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
            }
        }
    }
}
