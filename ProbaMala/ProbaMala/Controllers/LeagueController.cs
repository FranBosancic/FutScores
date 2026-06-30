using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Primary route: /lige (Croatian), English aliases: /leagues (named for asp-route use)
    // Authorization (per Lab5): Index + search are public ([AllowAnonymous]); Details is
    // visible to any signed-in user (inherits the class [Authorize]); create/edit/delete
    // are Admin-only.
    [Authorize]
    [Route("lige")]
    public class LeagueController : Controller
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ILogger<LeagueController> _logger;

        public LeagueController(ILeagueRepository leagueRepository, ILogger<LeagueController> logger)
        {
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        // GET /leagues
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/leagues", Name = "leagues-index")]
        [HttpGet("~/leagues/list")]
        [AllowAnonymous]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_leagueRepository.GetAll(q));
        }

        // GET /leagues/filter  (AJAX — returns the _LeagueList partial, not the full page)
        [HttpGet("filter")]
        [HttpGet("~/leagues/filter", Name = "leagues-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_LeagueList", _leagueRepository.GetAll(q));
        }

        // GET /leagues/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/leagues/{id:int}", Name = "league-details")]
        [HttpGet("~/leagues/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var league = _leagueRepository.GetById(id);

            if (league == null)
                return NotFound();

            return View(league);
        }

        // GET /leagues/create
        [Authorize(Roles = "Admin")]
        [HttpGet("novo")]
        [HttpGet("~/leagues/create", Name = "league-create")]
        public IActionResult Create()
        {
            return View(_leagueRepository.BuildFormModel());
        }

        // POST /leagues/create
        [Authorize(Roles = "Admin")]
        [HttpPost("novo")]
        [HttpPost("~/leagues/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LeagueFormViewModel model)
        {
            ValidateLeagueForm(model);

            if (!ModelState.IsValid)
                return View(model);

            var leagueId = _leagueRepository.Add(model);
            _logger.LogInformation("League {LeagueId} created by {User}.", leagueId, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id = leagueId });
        }

        // GET /leagues/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/leagues/edit/{id:int}", Name = "league-edit")]
        public IActionResult Edit(int id)
        {
            var model = _leagueRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /leagues/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/leagues/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LeagueFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            ValidateLeagueForm(model);

            if (!ModelState.IsValid)
                return View(model);

            var updated = _leagueRepository.Update(id, model);

            if (!updated)
                return NotFound();

            _logger.LogInformation("League {LeagueId} updated by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /leagues/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/leagues/delete/{id:int}", Name = "league-delete")]
        public IActionResult Delete(int id)
        {
            var model = _leagueRepository.GetById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /leagues/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/leagues/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _leagueRepository.Delete(id);

            if (!deleted)
                return NotFound();

            _logger.LogInformation("League {LeagueId} deleted by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        // Checks that the name is unique (case-insensitive).
        // Passing the current id ensures the check doesn't flag the league's own name during Edit.
        private void ValidateLeagueForm(LeagueFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) && _leagueRepository.NameExists(model.Name, model.Id == 0 ? null : model.Id))
                ModelState.AddModelError(nameof(model.Name), "A league with this name already exists.");
        }
    }
}
