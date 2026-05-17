using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("klubovi")]
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;

        public ClubController(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/clubs", Name = "clubs-index")]
        [HttpGet("~/clubs/list")]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_clubRepository.GetAll(q));
        }

        [HttpGet("filter")]
        [HttpGet("~/clubs/filter", Name = "clubs-filter")]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_ClubList", _clubRepository.GetAll(q));
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/clubs/{id:int}", Name = "club-details")]
        [HttpGet("~/clubs/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _clubRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet("novo")]
        [HttpGet("~/clubs/create", Name = "club-create")]
        public IActionResult Create()
        {
            return View(_clubRepository.BuildFormModel());
        }

        [HttpPost("novo")]
        [HttpPost("~/clubs/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClubFormViewModel model)
        {
            ValidateClubForm(model);

            if (!ModelState.IsValid)
            {
                _clubRepository.PopulateFormOptions(model);
                return View(model);
            }

            var clubId = _clubRepository.Add(model);
            return RedirectToAction(nameof(Details), new { id = clubId });
        }

        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/clubs/edit/{id:int}", Name = "club-edit")]
        public IActionResult Edit(int id)
        {
            var model = _clubRepository.GetFormById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/clubs/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ClubFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            ValidateClubForm(model);

            if (!ModelState.IsValid)
            {
                _clubRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _clubRepository.Update(id, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/clubs/delete/{id:int}", Name = "club-delete")]
        public IActionResult Delete(int id)
        {
            var model = _clubRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost("obrisi/{id:int}")]
        [HttpPost("~/clubs/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_clubRepository.CanDelete(id))
            {
                var model = _clubRepository.GetById(id);

                if (model == null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, "This club cannot be deleted while it still has related players or matches.");
                return View(model);
            }

            var deleted = _clubRepository.Delete(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private void ValidateClubForm(ClubFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) && _clubRepository.NameExists(model.Name, model.Id == 0 ? null : model.Id))
            {
                ModelState.AddModelError(nameof(model.Name), "A club with this name already exists.");
            }

            if (model.LeagueId.HasValue && !_clubRepository.LeagueExists(model.LeagueId.Value))
            {
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");
            }
        }
    }
}
