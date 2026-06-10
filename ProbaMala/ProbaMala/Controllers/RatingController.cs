using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Mutations are Admin-only; the read actions (incl. cascade JSON helpers)
    // opt back in with [AllowAnonymous].
    [Authorize(Roles = "Admin")]
    [Route("ocjene")]
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
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
            return View(_ratingRepository.GetAll(q));
        }

        // GET /ratings/filter  (AJAX — returns only the list partial)
        [HttpGet("filter")]
        [HttpGet("~/ratings/filter", Name = "ratings-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_RatingList", _ratingRepository.GetAll(q));
        }

        // GET /ratings/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/ratings/{id:int}", Name = "rating-details")]
        [HttpGet("~/ratings/details/{id:int}")]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // GET /ratings/create?matchId=5&playerId=12
        // The matchId and playerId query params come from the "Rate" shortcut button
        // on the match squad page — they pre-fill the cascade so the rater only has
        // to enter a score and comment.
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

        // POST /ratings/create
        [HttpPost("novo")]
        [HttpPost("~/ratings/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RatingFormViewModel model)
        {
            ValidateRatingForm(model);

            if (!ModelState.IsValid)
            {
                _ratingRepository.PopulateFormOptions(model);
                return View(model);
            }

            var ratingId = _ratingRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = ratingId });
        }

        // GET /ratings/edit/{id}
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/ratings/edit/{id:int}", Name = "rating-edit")]
        public IActionResult Edit(int id)
        {
            var model = _ratingRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /ratings/edit/{id}
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/ratings/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RatingFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            ValidateRatingForm(model);

            if (!ModelState.IsValid)
            {
                _ratingRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _ratingRepository.Update(id, model);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /ratings/delete/{id}
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/ratings/delete/{id:int}", Name = "rating-delete")]
        public IActionResult Delete(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // POST /ratings/delete/{id}
        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/ratings/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _ratingRepository.Delete(id);

            if (!deleted)
                return NotFound();

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
