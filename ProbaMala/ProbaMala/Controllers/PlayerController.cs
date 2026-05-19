using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("igraci")]
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/players", Name = "players-index")]
        [HttpGet("~/players/list")]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_playerRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/players/filter", Name = "players-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_PlayerList", _playerRepository.GetAll(q));
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/players/{id:int}", Name = "player-details")]
        [HttpGet("~/players/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _playerRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet("novo")]
        [HttpGet("~/players/create", Name = "player-create")]
        public IActionResult Create()
        {
            return View(_playerRepository.BuildFormModel());
        }

        [HttpPost("novo")]
        [HttpPost("~/players/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PlayerFormViewModel model)
        {
            ValidatePlayerForm(model);

            if (!ModelState.IsValid)
            {
                _playerRepository.PopulateFormOptions(model);
                return View(model);
            }

            var playerId = _playerRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = playerId });
        }

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/players/edit/{id:int}", Name = "player-edit")]
        public IActionResult Edit(int id)
        {
            var model = _playerRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/players/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PlayerFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidatePlayerForm(model);

            if (!ModelState.IsValid)
            {
                _playerRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _playerRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/players/delete/{id:int}", Name = "player-delete")]
        public IActionResult Delete(int id)
        {
            var model = _playerRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/players/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _playerRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidatePlayerForm(PlayerFormViewModel model)
        {
            if (model.ClubId.HasValue && !_playerRepository.ClubExists(model.ClubId.Value))
            {
                ModelState.AddModelError(nameof(model.ClubId), "The selected club does not exist.");
            }
        }
    }
}
