using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Primary route: /klubovi (Croatian), English aliases: /clubs
    // Authorization (per Lab5): Index + search are public ([AllowAnonymous]); Details is
    // visible to any signed-in user (inherits the class [Authorize]); create/edit/delete
    // and image management are Admin-only.
    [Authorize]
    [Route("klubovi")]
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IImageRepository _imageRepository;

        public ClubController(IClubRepository clubRepository, IImageRepository imageRepository)
        {
            _clubRepository = clubRepository;
            _imageRepository = imageRepository;
        }

        // GET /clubs  — optional text filter (q) and league filter (leagueId)
        // leagueId is set when navigating from the league nav dropdown
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/clubs", Name = "clubs-index")]
        [HttpGet("~/clubs/list")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public IActionResult Filter(string? q, int? leagueId)
        {
            ViewData["FilterQuery"] = q;
            ViewData["LeagueId"]    = leagueId;
            return PartialView("_ClubList", _clubRepository.GetAll(q, leagueId));
        }

        // GET /clubs/{id}/squad  (AJAX — returns the _ClubSquad partial filtered by query)
        [HttpGet("{id:int}/posada")]
        [HttpGet("~/clubs/{id:int}/squad", Name = "club-squad-filter")]
        [AllowAnonymous]
        public IActionResult FilterSquad(int id, string? q)
        {
            ViewData["FilterQuery"] = q;
            ViewData["ClubId"]      = id;
            return PartialView("_ClubSquad", _clubRepository.GetSquad(id, q));
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
        [Authorize(Roles = "Admin")]
        [HttpGet("novo")]
        [HttpGet("~/clubs/create", Name = "club-create")]
        public IActionResult Create()
        {
            return View(_clubRepository.BuildFormModel());
        }

        // POST /clubs/create
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        // ── Image upload (Dropzone) ──────────────────────────────────────────

        // POST /clubs/{id}/images — Dropzone posts one file per request as "file".
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/slike")]
        [HttpPost("~/clubs/{id:int}/images")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var (image, error) = await _imageRepository.AddAsync(ImageOwnerType.Club, id, file);

            if (error != null)
                return BadRequest(error);

            return Json(new { success = true, id = image!.Id });
        }

        // GET /clubs/{id}/images — AJAX: renders the gallery partial.
        [HttpGet("{id:int}/slike")]
        [HttpGet("~/clubs/{id:int}/images")]
        [AllowAnonymous]
        public IActionResult GetImages(int id)
        {
            ViewData["PrimaryNoun"] = "banner";
            return PartialView("_ImageList", _imageRepository.GetForOwner(ImageOwnerType.Club, id));
        }

        // POST /clubs/images/delete — imageId comes from the AJAX request body.
        [Authorize(Roles = "Admin")]
        [HttpPost("slike/obrisi")]
        [HttpPost("~/clubs/images/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int imageId)
        {
            return _imageRepository.Delete(imageId) ? Json(new { success = true }) : NotFound();
        }

        // POST /clubs/images/primary — mark as the club banner.
        [Authorize(Roles = "Admin")]
        [HttpPost("slike/glavna")]
        [HttpPost("~/clubs/images/primary")]
        [ValidateAntiForgeryToken]
        public IActionResult SetPrimaryImage(int imageId)
        {
            return _imageRepository.SetPrimary(imageId) ? Json(new { success = true }) : NotFound();
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
