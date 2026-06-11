using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // The [Route] attribute on the class sets the base URL segment for all actions.
    // Each action below also has a Croatian and an English alias route; the English
    // ones are named (Name = "match-details" etc.) so views can reference them with
    // asp-route="match-details" instead of hardcoding the URL.
    // Authorization (per Lab5): Index + search (and the cascade JSON helper) are public
    // ([AllowAnonymous]); Details is visible to any signed-in user (inherits the class
    // [Authorize]); create/edit/delete are Admin-only.
    [Authorize]
    [Route("utakmice")]
    public class MatchController : Controller
    {
        private readonly IMatchRepository _matchRepository;

        // IMatchRepository is injected by the DI container (configured in Program.cs).
        // The controller doesn't know or care whether the data comes from a real DB or a mock.
        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        // GET /matches  or  /utakmice
        // Accepts an optional text filter (q) and an optional league filter (leagueId).
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/matches", Name = "matches-index")]
        [HttpGet("~/matches/list")]
        [AllowAnonymous]
        public IActionResult Index(string? q, int? leagueId)
        {
            ViewData["FilterQuery"] = q;
            ViewData["LeagueId"]    = leagueId;
            return View(_matchRepository.GetAll(q, leagueId));
        }

        // GET /matches/filter  (AJAX — returns the _MatchList partial, not the full page)
        // The live-search input calls this endpoint and replaces the list area with the result.
        [HttpGet("filter")]
        [HttpGet("~/matches/filter", Name = "matches-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q, int? leagueId)
        {
            ViewData["FilterQuery"] = q;
            ViewData["LeagueId"]    = leagueId;
            return PartialView("_MatchList", _matchRepository.GetAll(q, leagueId));
        }

        // GET /matches/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/matches/{id:int}", Name = "match-details")]
        [HttpGet("~/matches/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _matchRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // GET /matches/create  — shows the blank form
        [Authorize(Roles = "Admin")]
        [HttpGet("novo")]
        [HttpGet("~/matches/create", Name = "match-create")]
        public IActionResult Create()
        {
            return View(_matchRepository.BuildFormModel());
        }

        // GET /matches/clubs?leagueId=5  (AJAX — JSON)
        // Used by the cascade dropdowns: when the user picks a league, this endpoint
        // returns the clubs in that league so the home/away selects can be populated.
        [HttpGet("klubovi")]
        [HttpGet("~/matches/clubs", Name = "match-clubs")]
        [AllowAnonymous]
        public IActionResult ClubsInLeague(int leagueId, int? excludeId)
        {
            return Json(_matchRepository.GetClubsInLeague(leagueId, excludeId));
        }

        // POST /matches/create  — processes the submitted form
        [Authorize(Roles = "Admin")]
        [HttpPost("novo")]
        [HttpPost("~/matches/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MatchFormViewModel model)
        {
            // Server-side business-rule validation (on top of the [Required] attribute checks)
            ValidateMatchForm(model);

            if (!ModelState.IsValid)
            {
                // Re-populate the dropdown options before re-rendering the form,
                // because the option lists aren't submitted with the form.
                _matchRepository.PopulateFormOptions(model);
                return View(model);
            }

            var matchId = _matchRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = matchId });
        }

        // GET /matches/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/matches/edit/{id:int}", Name = "match-edit")]
        public IActionResult Edit(int id)
        {
            var model = _matchRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /matches/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/matches/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MatchFormViewModel model)
        {
            // Extra safety check: the id in the URL must match the id in the form body
            if (id != model.Id)
                return BadRequest();

            ValidateMatchForm(model);

            if (!ModelState.IsValid)
            {
                _matchRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _matchRepository.Update(id, model);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /matches/delete/{id}  — shows the confirmation page
        [Authorize(Roles = "Admin")]
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/matches/delete/{id:int}", Name = "match-delete")]
        public IActionResult Delete(int id)
        {
            var model = _matchRepository.GetById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /matches/delete/{id}  — performs the actual delete after confirmation
        // [ActionName] lets the GET and POST share the same URL while having different
        // method names in C# (avoids the compile error from identical signatures).
        [HttpPost("obrisi/{id:int}")]
        [Authorize(Roles = "Admin")]
        [HttpPost("~/matches/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _matchRepository.Delete(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        // Adds ModelState errors for business rules that can't be expressed with
        // simple data annotations (e.g. "both clubs must belong to the same league").
        private void ValidateMatchForm(MatchFormViewModel model)
        {
            if (model.LeagueId.HasValue && !_matchRepository.LeagueExists(model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");

            if (model.HomeTeamId.HasValue && !_matchRepository.ClubExists(model.HomeTeamId.Value))
                ModelState.AddModelError(nameof(model.HomeTeamId), "The selected home team does not exist.");

            if (model.AwayTeamId.HasValue && !_matchRepository.ClubExists(model.AwayTeamId.Value))
                ModelState.AddModelError(nameof(model.AwayTeamId), "The selected away team does not exist.");

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.HomeTeamId == model.AwayTeamId)
                ModelState.AddModelError(nameof(model.AwayTeamId), "Home and away team must be different clubs.");

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue
                && !_matchRepository.ClubBelongsToLeague(model.HomeTeamId.Value, model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.HomeTeamId), "The home team does not belong to the chosen league.");

            if (model.LeagueId.HasValue && model.AwayTeamId.HasValue
                && !_matchRepository.ClubBelongsToLeague(model.AwayTeamId.Value, model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.AwayTeamId), "The away team does not belong to the chosen league.");
        }
    }
}
