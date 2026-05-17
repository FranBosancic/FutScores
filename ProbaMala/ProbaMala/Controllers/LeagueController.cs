using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("lige")]
    public class LeagueController : Controller
    {
        private readonly ILeagueRepository _leagueRepository;

        public LeagueController(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/leagues", Name = "leagues-index")]
        [HttpGet("~/leagues/list")]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_leagueRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/leagues/filter", Name = "leagues-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_LeagueList", _leagueRepository.GetAll(q));
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/leagues/{id:int}", Name = "league-details")]
        [HttpGet("~/leagues/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var league = _leagueRepository.GetById(id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        [HttpGet("novo")]
        [HttpGet("~/leagues/create", Name = "league-create")]
        public IActionResult Create()
        {
            return View(_leagueRepository.BuildFormModel());
        }

        [HttpPost("novo")]
        [HttpPost("~/leagues/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LeagueFormViewModel model)
        {
            ValidateLeagueForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var leagueId = _leagueRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = leagueId });
        }

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/leagues/edit/{id:int}", Name = "league-edit")]
        public IActionResult Edit(int id)
        {
            var model = _leagueRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/leagues/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LeagueFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidateLeagueForm(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = _leagueRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/leagues/delete/{id:int}", Name = "league-delete")]
        public IActionResult Delete(int id)
        {
            var model = _leagueRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/leagues/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _leagueRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateLeagueForm(LeagueFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) && _leagueRepository.NameExists(model.Name, model.Id == 0 ? null : model.Id))
            {
                ModelState.AddModelError(nameof(model.Name), "A league with this name already exists.");
            }
        }
    }
}
