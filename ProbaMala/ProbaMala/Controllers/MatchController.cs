using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;
using ProbaMala.Services;

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
        private readonly IAiDataEntryService _aiService;
        private readonly INameResolver _resolver;
        private readonly ILogger<MatchController> _logger;

        // IMatchRepository is injected by the DI container (configured in Program.cs).
        // The controller doesn't know or care whether the data comes from a real DB or a mock.
        public MatchController(
            IMatchRepository matchRepository,
            IAiDataEntryService aiService,
            INameResolver resolver,
            ILogger<MatchController> logger)
        {
            _matchRepository = matchRepository;
            _aiService = aiService;
            _resolver = resolver;
            _logger = logger;
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
            ViewData["AiConfigured"] = _aiService.IsConfigured;
            return View(_matchRepository.BuildFormModel());
        }

        // POST /matches/ai — AI-assisted pre-fill (Admin only). Extracts the fixture from a
        // natural-language note, resolves both club names to ids (league derived from the
        // home club), and returns the Create form pre-filled for review. Writes nothing.
        [Authorize(Roles = "Admin")]
        [HttpPost("ai")]
        [HttpPost("~/matches/ai")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AiFill(string prompt)
        {
            ViewData["AiConfigured"] = _aiService.IsConfigured;

            if (!_aiService.IsConfigured || string.IsNullOrWhiteSpace(prompt))
            {
                ModelState.AddModelError(string.Empty,
                    _aiService.IsConfigured ? "Describe the match for the AI first." : "The AI assistant is not configured.");
                return View("Create", _matchRepository.BuildFormModel());
            }

            var result = await _aiService.ExtractMatchAsync(prompt);
            if (!result.Success || result.Value is null)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "The AI couldn't understand that. Try rephrasing.");
                return View("Create", _matchRepository.BuildFormModel());
            }

            var intent = result.Value;
            var model = new MatchFormViewModel
            {
                HomeGoals = Math.Clamp(intent.HomeGoals, 0, 99),
                AwayGoals = Math.Clamp(intent.AwayGoals, 0, 99)
            };

            if (AiParsing.TryParseFlexibleDate(intent.Date, out var date))
                model.Date = date;

            // Resolve both clubs; the league comes from the home club (matches live in one league).
            var home = _resolver.ResolveClub(intent.HomeTeamName);
            var away = _resolver.ResolveClub(intent.AwayTeamName);
            if (home != null) { model.LeagueId = home.LeagueId; model.HomeTeamId = home.Id; }
            if (away != null) model.AwayTeamId = away.Id;

            _matchRepository.PopulateFormOptions(model);

            ViewData["AiNote"] = home != null && away != null
                ? "Pre-filled by AI — review the details and save."
                : $"AI read “{intent.HomeTeamName} vs {intent.AwayTeamName}” but couldn't match both clubs — please pick them.";

            _logger.LogInformation("AI pre-filled a match form for {User}.", User.Identity?.Name);
            return View("Create", model);
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
            _logger.LogInformation("Match {MatchId} created by {User}.", matchId, User.Identity?.Name);
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

            _logger.LogInformation("Match {MatchId} updated by {User}.", id, User.Identity?.Name);
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

            _logger.LogInformation("Match {MatchId} deleted by {User}.", id, User.Identity?.Name);
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
