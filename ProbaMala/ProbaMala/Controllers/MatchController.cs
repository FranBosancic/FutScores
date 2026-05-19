using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("utakmice")]
    public class MatchController : Controller
    {
        private readonly IMatchRepository _matchRepository;

        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/matches", Name = "matches-index")]
        [HttpGet("~/matches/list")]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_matchRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/matches/filter", Name = "matches-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_MatchList", _matchRepository.GetAll(q));
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/matches/{id:int}", Name = "match-details")]
        [HttpGet("~/matches/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _matchRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet("novo")]
        [HttpGet("~/matches/create", Name = "match-create")]
        public IActionResult Create()
        {
            return View(_matchRepository.BuildFormModel());
        }

        [HttpPost("novo")]
        [HttpPost("~/matches/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MatchFormViewModel model)
        {
            ValidateMatchForm(model);

            if (!ModelState.IsValid)
            {
                _matchRepository.PopulateFormOptions(model);
                return View(model);
            }

            var matchId = _matchRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = matchId });
        }

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/matches/edit/{id:int}", Name = "match-edit")]
        public IActionResult Edit(int id)
        {
            var model = _matchRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/matches/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MatchFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidateMatchForm(model);

            if (!ModelState.IsValid)
            {
                _matchRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _matchRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/matches/delete/{id:int}", Name = "match-delete")]
        public IActionResult Delete(int id)
        {
            var model = _matchRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/matches/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _matchRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateMatchForm(MatchFormViewModel model)
        {
            if (model.LeagueId.HasValue && !_matchRepository.LeagueExists(model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");
            }

            if (model.HomeTeamId.HasValue && !_matchRepository.ClubExists(model.HomeTeamId.Value))
            {
                ModelState.AddModelError(nameof(model.HomeTeamId), "The selected home team does not exist.");
            }

            if (model.AwayTeamId.HasValue && !_matchRepository.ClubExists(model.AwayTeamId.Value))
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "The selected away team does not exist.");
            }

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue && model.HomeTeamId.Value == model.AwayTeamId.Value)
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "Home and away team must be different clubs.");
            }

            if (model.LeagueId.HasValue && model.HomeTeamId.HasValue && !_matchRepository.ClubBelongsToLeague(model.HomeTeamId.Value, model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.HomeTeamId), "The selected home team does not belong to the chosen league.");
            }

            if (model.LeagueId.HasValue && model.AwayTeamId.HasValue && !_matchRepository.ClubBelongsToLeague(model.AwayTeamId.Value, model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.AwayTeamId), "The selected away team does not belong to the chosen league.");
            }
        }
    }
}
