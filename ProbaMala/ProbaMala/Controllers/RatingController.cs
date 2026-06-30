using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Authorization: Index + search (and the cascade JSON helpers) are public
    // ([AllowAnonymous]); Details and creating a rating are open to any signed-in user
    // (submitting a rating is the app's core user action, so they inherit the class
    // [Authorize]); editing/deleting a rating is allowed for its OWNER or an Admin.
    [Authorize]
    [Route("ocjene")]
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RatingController> _logger;

        public RatingController(
            IRatingRepository ratingRepository,
            UserManager<AppUser> userManager,
            ILogger<RatingController> logger)
        {
            _ratingRepository = ratingRepository;
            _userManager = userManager;
            _logger = logger;
        }

        // The rating-author profile id tied to the signed-in user, or null when
        // anonymous / not yet linked. Drives ownership decisions below.
        private int? CurrentProfileId()
        {
            var appUserId = _userManager.GetUserId(User);
            return appUserId == null ? null : _ratingRepository.GetProfileIdForAppUser(appUserId);
        }

        // A rating may be edited/deleted by an Admin or by the user who authored it.
        private bool CanModify(int ratingAuthorId) =>
            User.IsInRole(IdentitySeeder.AdminRole)
            || (CurrentProfileId() is int profileId && profileId == ratingAuthorId);

        // Exposes ownership info to the list/details views so they can show Edit/Delete
        // only on ratings the current user is allowed to change.
        private void SetOwnershipViewData()
        {
            ViewData["IsAdmin"] = User.IsInRole(IdentitySeeder.AdminRole);
            ViewData["CurrentProfileId"] = CurrentProfileId();
        }

        // GET /ratings
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/ratings", Name = "ratings-index")]
        [HttpGet("~/ratings/list")]
        [AllowAnonymous]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            SetOwnershipViewData();
            return View(_ratingRepository.GetAll(q));
        }

        // GET /ratings/filter  (AJAX — returns only the list partial)
        [HttpGet("filter")]
        [HttpGet("~/ratings/filter", Name = "ratings-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            SetOwnershipViewData();
            return PartialView("_RatingList", _ratingRepository.GetAll(q));
        }

        // GET /ratings/mine — the signed-in user's own ratings, with edit/delete controls.
        // ("YOU" link in the header points here.)
        [HttpGet("moje")]
        [HttpGet("~/ratings/mine", Name = "ratings-mine")]
        public IActionResult Mine()
        {
            SetOwnershipViewData();
            ViewData["RatingListTitle"] = "Your ratings";
            ViewData["RatingActionsAlwaysVisible"] = true;

            var profileId = CurrentProfileId();
            var ratings = profileId.HasValue
                ? _ratingRepository.GetByUserId(profileId.Value)
                : new List<RatingDetailsViewModel>();

            return View(ratings);
        }

        // GET /ratings/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/ratings/{id:int}", Name = "rating-details")]
        [HttpGet("~/ratings/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            SetOwnershipViewData();
            return View(viewModel);
        }

        // GET /ratings/create?matchId=5&playerId=12
        // The matchId and playerId query params come from the "Rate" shortcut button
        // on the match squad page — they pre-fill the cascade so the rater only has
        // to enter a score and comment.
        // Any signed-in user may open the rating form (inherits the class [Authorize]).
        [HttpGet("novo")]
        [HttpGet("~/ratings/create", Name = "rating-create")]
        public IActionResult Create(int? matchId, int? playerId)
        {
            return View(_ratingRepository.BuildFormModel(matchId, playerId));
        }

        // ── Cascade JSON endpoints ────────────────────────────────────────────
        // The rating form has five dependent dropdowns: League → Home → Away → Match → Player.
        // Changing one step triggers an AJAX call to one of these endpoints to rebuild the
        // next step's options. The JavaScript in site.js handles the wiring.

        // GET /ratings/clubs?leagueId=5
        [HttpGet("klubovi")]
        [HttpGet("~/ratings/clubs", Name = "rating-clubs")]
        [AllowAnonymous]
        public IActionResult ClubsInLeague(int leagueId, int? excludeId)
        {
            return Json(_ratingRepository.GetClubsInLeague(leagueId, excludeId));
        }

        // GET /ratings/matches?homeTeamId=1&awayTeamId=3
        [HttpGet("utakmice")]
        [HttpGet("~/ratings/matches", Name = "rating-matches")]
        [AllowAnonymous]
        public IActionResult MatchesBetween(int homeTeamId, int awayTeamId)
        {
            return Json(_ratingRepository.GetMatchesBetween(homeTeamId, awayTeamId));
        }

        // GET /ratings/players?matchId=7
        [HttpGet("igraci")]
        [HttpGet("~/ratings/players", Name = "rating-players")]
        [AllowAnonymous]
        public IActionResult PlayersForMatch(int matchId)
        {
            return Json(_ratingRepository.GetPlayersForMatch(matchId));
        }

        // POST /ratings/create — any signed-in user may submit a rating. The author is
        // forced to the current user's own profile; it is never taken from the form.
        [HttpPost("novo")]
        [HttpPost("~/ratings/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RatingFormViewModel model)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
                return Challenge();

            model.UserId = _ratingRepository.GetOrCreateProfileId(
                appUser.Id, appUser.Email ?? appUser.UserName ?? appUser.Id);

            ValidateRatingForm(model);

            if (!ModelState.IsValid)
            {
                _ratingRepository.PopulateFormOptions(model);
                return View(model);
            }

            var ratingId = _ratingRepository.Add(model);
            _logger.LogInformation(
                "Rating {RatingId} created by profile {ProfileId}: player {PlayerId}, match {MatchId}, score {Score}.",
                ratingId, model.UserId, model.PlayerId, model.MatchId, model.Score);
            return RedirectToAction(nameof(Details), new { id = ratingId });
        }

        // GET /ratings/edit/{id} — the rating's author or an Admin.
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/ratings/edit/{id:int}", Name = "rating-edit")]
        public IActionResult Edit(int id)
        {
            var model = _ratingRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            if (!CanModify(model.UserId!.Value))
                return Forbid();

            return View(model);
        }

        // POST /ratings/edit/{id} — the rating's author or an Admin; the author is kept.
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/ratings/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RatingFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var existing = _ratingRepository.GetFormById(id);
            if (existing == null)
                return NotFound();

            if (!CanModify(existing.UserId!.Value))
                return Forbid();

            // An edit can't reassign authorship — keep the original author.
            model.UserId = existing.UserId;

            ValidateRatingForm(model);

            if (!ModelState.IsValid)
            {
                _ratingRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _ratingRepository.Update(id, model);

            if (!updated)
                return NotFound();

            _logger.LogInformation("Rating {RatingId} edited by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /ratings/delete/{id} — the rating's author or an Admin.
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/ratings/delete/{id:int}", Name = "rating-delete")]
        public IActionResult Delete(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            if (!CanModify(viewModel.UserId))
                return Forbid();

            return View(viewModel);
        }

        // POST /ratings/delete/{id} — the rating's author or an Admin.
        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/ratings/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var existing = _ratingRepository.GetById(id);
            if (existing == null)
                return NotFound();

            if (!CanModify(existing.UserId))
            {
                _logger.LogWarning("{User} was forbidden from deleting rating {RatingId}.", User.Identity?.Name, id);
                return Forbid();
            }

            _ratingRepository.Delete(id);
            _logger.LogInformation("Rating {RatingId} deleted by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        // Validates the rating form beyond what data annotations can express.
        // We walk the whole League → Home → Away → Match → Player hierarchy and
        // make sure every selected value is consistent with the one above it.
        // This guards against stale or tampered form data that bypasses the cascade UI.
        private void ValidateRatingForm(RatingFormViewModel model)
        {
            if (model.LeagueId.HasValue && !_ratingRepository.LeagueExists(model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue
                && !_ratingRepository.ClubInLeague(model.HomeTeamId.Value, model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.HomeTeamId), "The home team is not part of the selected league.");

            if (model.LeagueId.HasValue && model.AwayTeamId.HasValue
                && !_ratingRepository.ClubInLeague(model.AwayTeamId.Value, model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.AwayTeamId), "The away team is not part of the selected league.");

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.HomeTeamId == model.AwayTeamId)
                ModelState.AddModelError(nameof(model.AwayTeamId), "The home and away teams must be different.");

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.MatchId.HasValue
                && !_ratingRepository.MatchHasTeams(model.MatchId.Value, model.LeagueId.Value, model.HomeTeamId.Value, model.AwayTeamId.Value))
                ModelState.AddModelError(nameof(model.MatchId), "The selected match doesn't belong to the chosen league and teams.");

            if (model.PlayerId.HasValue && !_ratingRepository.PlayerExists(model.PlayerId.Value))
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player does not exist.");

            if (model.PlayerId.HasValue && model.MatchId.HasValue
                && !_ratingRepository.IsPlayerInMatch(model.PlayerId.Value, model.MatchId.Value))
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player did not play for either club in the chosen match.");

            if (model.UserId.HasValue && !_ratingRepository.UserExists(model.UserId.Value))
                ModelState.AddModelError(nameof(model.UserId), "The selected user does not exist.");
        }
    }
}
