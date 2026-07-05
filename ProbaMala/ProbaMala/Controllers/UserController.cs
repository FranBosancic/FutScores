using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;
using ProbaMala.Services;

namespace ProbaMala.Controllers
{
    // Primary route: /korisnici (Croatian), English aliases: /users
    // Authorization (per Lab5): Index + search are public ([AllowAnonymous]); Details is
    // visible to any signed-in user (inherits the class [Authorize]); create/edit/delete
    // are Admin-only.
    [Authorize]
    [Route("korisnici")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAiDataEntryService _aiService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserRepository userRepository,
            IAiDataEntryService aiService,
            ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _aiService = aiService;
            _logger = logger;
        }

        // GET /users
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/users", Name = "users-index")]
        [HttpGet("~/users/list")]
        [AllowAnonymous]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_userRepository.GetAll(q));
        }

        // GET /users/filter  (AJAX — returns the _UserList partial)
        [HttpGet("filter")]
        [HttpGet("~/users/filter", Name = "users-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_UserList", _userRepository.GetAll(q));
        }

        // GET /users/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/users/{id:int}", Name = "user-details")]
        [HttpGet("~/users/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET /users/create
        [Authorize(Roles = "Admin")]
        [HttpGet("novo")]
        [HttpGet("~/users/create", Name = "user-create")]
        public IActionResult Create()
        {
            ViewData["AiConfigured"] = _aiService.IsConfigured;
            return View(_userRepository.BuildFormModel());
        }

        // POST /users/ai — AI-assisted pre-fill (Admin only). Extracts name + email from a
        // natural-language note and returns the Create form pre-filled for review. No FK
        // resolution needed; writes nothing itself.
        [Authorize(Roles = "Admin")]
        [HttpPost("ai")]
        [HttpPost("~/users/ai")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AiFill(string prompt)
        {
            ViewData["AiConfigured"] = _aiService.IsConfigured;

            if (!_aiService.IsConfigured || string.IsNullOrWhiteSpace(prompt))
            {
                ModelState.AddModelError(string.Empty,
                    _aiService.IsConfigured ? "Describe the user for the AI first." : "The AI assistant is not configured.");
                return View("Create", _userRepository.BuildFormModel());
            }

            var result = await _aiService.ExtractUserAsync(prompt);
            if (!result.Success || result.Value is null)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "The AI couldn't understand that. Try rephrasing.");
                return View("Create", _userRepository.BuildFormModel());
            }

            var intent = result.Value;
            var model = new UserFormViewModel
            {
                FirstName = intent.FirstName,
                LastName  = intent.LastName,
                Email     = intent.Email
            };

            ViewData["AiNote"] = "Pre-filled by AI — review the details and save.";
            _logger.LogInformation("AI pre-filled a user form for {User}.", User.Identity?.Name);
            return View("Create", model);
        }

        // POST /users/create
        [Authorize(Roles = "Admin")]
        [HttpPost("novo")]
        [HttpPost("~/users/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFormViewModel model)
        {
            ValidateUserForm(model);

            if (!ModelState.IsValid)
                return View(model);

            var userId = _userRepository.Add(model);
            _logger.LogInformation("User {UserId} created by {User}.", userId, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        // GET /users/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/users/edit/{id:int}", Name = "user-edit")]
        public IActionResult Edit(int id)
        {
            var model = _userRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /users/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/users/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            ValidateUserForm(model);

            if (!ModelState.IsValid)
                return View(model);

            var updated = _userRepository.Update(id, model);

            if (!updated)
                return NotFound();

            _logger.LogInformation("User {UserId} updated by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /users/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/users/delete/{id:int}", Name = "user-delete")]
        public IActionResult Delete(int id)
        {
            var model = _userRepository.GetById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /users/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/users/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _userRepository.Delete(id);

            if (!deleted)
                return NotFound();

            _logger.LogInformation("User {UserId} deleted by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        // Checks that the email address is unique (case-insensitive).
        private void ValidateUserForm(UserFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && _userRepository.EmailExists(model.Email, model.Id == 0 ? null : model.Id))
                ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
        }
    }
}
