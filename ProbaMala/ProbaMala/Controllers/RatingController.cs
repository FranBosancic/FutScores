using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("ocjene")]
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/ratings", Name = "ratings-index")]
        [HttpGet("~/ratings/list")]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_ratingRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/ratings/filter", Name = "ratings-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_RatingList", _ratingRepository.GetAll(q));
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/ratings/{id:int}", Name = "rating-details")]
        [HttpGet("~/ratings/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet("novo")]
        [HttpGet("~/ratings/create", Name = "rating-create")]
        public IActionResult Create(int? matchId, int? playerId)
        {
            // matchId / playerId arrive from the "Rate" shortcut on a match squad
            // and pre-fill the whole match context for the rater.
            return View(_ratingRepository.BuildFormModel(matchId, playerId));
        }

        // ── Cascade JSON endpoints (League → Home → Away → Match → Player) ──

        [HttpGet("klubovi")]
        [HttpGet("~/ratings/clubs", Name = "rating-clubs")]
        public IActionResult ClubsInLeague(int leagueId, int? excludeId)
        {
            return Json(_ratingRepository.GetClubsInLeague(leagueId, excludeId));
        }

        [HttpGet("utakmice")]
        [HttpGet("~/ratings/matches", Name = "rating-matches")]
        public IActionResult MatchesBetween(int homeTeamId, int awayTeamId)
        {
            return Json(_ratingRepository.GetMatchesBetween(homeTeamId, awayTeamId));
        }

        [HttpGet("igraci")]
        [HttpGet("~/ratings/players", Name = "rating-players")]
        public IActionResult PlayersForMatch(int matchId)
        {
            return Json(_ratingRepository.GetPlayersForMatch(matchId));
        }

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

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/ratings/edit/{id:int}", Name = "rating-edit")]
        public IActionResult Edit(int id)
        {
            var model = _ratingRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/ratings/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RatingFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidateRatingForm(model);

            if (!ModelState.IsValid)
            {
                _ratingRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _ratingRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/ratings/delete/{id:int}", Name = "rating-delete")]
        public IActionResult Delete(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/ratings/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _ratingRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateRatingForm(RatingFormViewModel model)
        {
            // Walk the hierarchy: every step must exist and be consistent with the
            // one above it. The [Required] attributes already cover "nothing picked";
            // here we guard against tampered or stale ids that bypass the cascade UI.

            if (model.LeagueId.HasValue && !_ratingRepository.LeagueExists(model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");
            }

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue && !_ratingRepository.ClubInLeague(model.HomeTeamId.Value, model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.HomeTeamId), "The home team is not part of the selected league.");
            }

            if (model.LeagueId.HasValue && model.AwayTeamId.HasValue && !_ratingRepository.ClubInLeague(model.AwayTeamId.Value, model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "The away team is not part of the selected league.");
            }

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.HomeTeamId == model.AwayTeamId)
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "The home and away teams must be different.");
            }

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.MatchId.HasValue
                && !_ratingRepository.MatchHasTeams(model.MatchId.Value, model.LeagueId.Value, model.HomeTeamId.Value, model.AwayTeamId.Value))
            {
                ModelState.AddModelError(nameof(model.MatchId), "The selected match doesn't belong to the chosen league and teams.");
            }

            if (model.PlayerId.HasValue && !_ratingRepository.PlayerExists(model.PlayerId.Value))
            {
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player does not exist.");
            }

            if (model.PlayerId.HasValue && model.MatchId.HasValue && !_ratingRepository.IsPlayerInMatch(model.PlayerId.Value, model.MatchId.Value))
            {
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player did not play for either club in the chosen match.");
            }

            if (model.UserId.HasValue && !_ratingRepository.UserExists(model.UserId.Value))
            {
                ModelState.AddModelError(nameof(model.UserId), "The selected user does not exist.");
            }
        }
    }
}
