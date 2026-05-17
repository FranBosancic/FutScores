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
        public IActionResult Create()
        {
            return View(_ratingRepository.BuildFormModel());
        }

        [HttpGet("igraci/pretraga")]
        [HttpGet("~/ratings/players/search", Name = "rating-player-search")]
        public IActionResult SearchPlayers(string? q)
        {
            return Json(_ratingRepository.SearchPlayers(q));
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
            if (model.PlayerId.HasValue && !_ratingRepository.PlayerExists(model.PlayerId.Value))
            {
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player does not exist.");
            }

            if (model.MatchId.HasValue && !_ratingRepository.MatchExists(model.MatchId.Value))
            {
                ModelState.AddModelError(nameof(model.MatchId), "The selected match does not exist.");
            }

            if (model.UserId.HasValue && !_ratingRepository.UserExists(model.UserId.Value))
            {
                ModelState.AddModelError(nameof(model.UserId), "The selected user does not exist.");
            }

            if (model.PlayerId.HasValue && model.MatchId.HasValue && !_ratingRepository.IsPlayerInMatch(model.PlayerId.Value, model.MatchId.Value))
            {
                ModelState.AddModelError(nameof(model.PlayerId), "The selected player is not tied to either club in the chosen match.");
            }
        }
    }
}
