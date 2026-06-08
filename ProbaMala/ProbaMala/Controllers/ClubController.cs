using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Primary route: /klubovi (Croatian), English aliases: /clubs
    [Route("klubovi")]
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;

        public ClubController(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        // GET /clubs  — optional text filter (q) and league filter (leagueId)
        // leagueId is set when navigating from the league nav dropdown
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/clubs", Name = "clubs-index")]
        [HttpGet("~/clubs/list")]
        public IActionResult Index(string? q, int? leagueId)
        {
            ViewData["FilterQuery"] = q;
            ViewData["LeagueId"]    = leagueId;
            return View(_clubRepository.GetAll(q, leagueId));
        }

        // GET /clubs/filter  (AJAX — returns the _ClubList partial)
        // leagueId must be forwarded so live search stays scoped to the league
        [HttpGet("filter")]
        [HttpGet("~/clubs/filter", Name = "clubs-filter")]
        public IActionResult Filter(string? q, int? leagueId)
        {
            ViewData["FilterQuery"] = q;
            ViewData["LeagueId"]    = leagueId;
            return PartialView("_ClubList", _clubRepository.GetAll(q, leagueId));
        }

        // GET /clubs/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/clubs/{id:int}", Name = "club-details")]
        [HttpGet("~/clubs/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _clubRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // GET /clubs/create
        [HttpGet("novo")]
        [HttpGet("~/clubs/create", Name = "club-create")]
        public IActionResult Create()
        {
            return View(_clubRepository.BuildFormModel());
        }

        // POST /clubs/create
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

        // GET /clubs/edit/{id}
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/clubs/edit/{id:int}", Name = "club-edit")]
        public IActionResult Edit(int id)
        {
            var model = _clubRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /clubs/edit/{id}
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/clubs/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ClubFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            ValidateClubForm(model);

            if (!ModelState.IsValid)
            {
                _clubRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _clubRepository.Update(id, model);

            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /clubs/delete/{id}
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/clubs/delete/{id:int}", Name = "club-delete")]
        public IActionResult Delete(int id)
        {
            var model = _clubRepository.GetById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /clubs/delete/{id}
        // A club can only be deleted when it has no players and no matches.
        // If those constraints aren't met we re-show the delete page with an error message.
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
                    return NotFound();

                ModelState.AddModelError(string.Empty, "This club cannot be deleted while it still has related players or matches.");
                return View(model);
            }

            var deleted = _clubRepository.Delete(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Checks that the name is unique and that the selected league exists.
        private void ValidateClubForm(ClubFormViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name) && _clubRepository.NameExists(model.Name, model.Id == 0 ? null : model.Id))
                ModelState.AddModelError(nameof(model.Name), "A club with this name already exists.");

            if (model.LeagueId.HasValue && !_clubRepository.LeagueExists(model.LeagueId.Value))
                ModelState.AddModelError(nameof(model.LeagueId), "The selected league does not exist.");
        }
    }
}
