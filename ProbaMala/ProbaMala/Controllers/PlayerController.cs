using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    // Authorization (per Lab5): Index + search are public ([AllowAnonymous]); Details is
    // visible to any signed-in user (inherits the class [Authorize]); create/edit/delete
    // and image management are Admin-only.
    [Authorize]
    [Route("igraci")]
    public class PlayerController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(
            IPlayerRepository playerRepository,
            IImageRepository imageRepository,
            ILogger<PlayerController> logger)
        {
            _playerRepository = playerRepository;
            _imageRepository = imageRepository;
            _logger = logger;
        }

        // GET /players
        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/players", Name = "players-index")]
        [HttpGet("~/players/list")]
        [AllowAnonymous]
        public IActionResult Index(string? q)
        {
            ViewData["FilterQuery"] = q;
            return View(_playerRepository.GetAll(q));
        }

        // GET /players/filter  (AJAX — returns only the list partial)
        [HttpGet("filter")]
        [HttpGet("~/players/filter", Name = "players-filter")]
        [AllowAnonymous]
        public IActionResult Filter(string? q)
        {
            ViewData["FilterQuery"] = q;
            return PartialView("_PlayerList", _playerRepository.GetAll(q));
        }

        // GET /players/{id}
        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/players/{id:int}", Name = "player-details")]
        [HttpGet("~/players/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _playerRepository.GetById(id);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // GET /players/create
        [Authorize(Roles = "Admin")]
        [HttpGet("novo")]
        [HttpGet("~/players/create", Name = "player-create")]
        public IActionResult Create()
        {
            return View(_playerRepository.BuildFormModel());
        }

        // GET /players/clubs?leagueId=5  (AJAX — JSON for the League → Club cascade)
        [HttpGet("klubovi")]
        [HttpGet("~/players/clubs", Name = "player-clubs")]
        [AllowAnonymous]
        public IActionResult ClubsInLeague(int leagueId)
        {
            return Json(_playerRepository.GetClubsInLeague(leagueId));
        }

        // POST /players/create
        [Authorize(Roles = "Admin")]
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
            _logger.LogInformation("Player {PlayerId} created by {User}.", playerId, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id = playerId });
        }

        // GET /players/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("uredi/{id:int}")]
        [HttpGet("~/players/edit/{id:int}", Name = "player-edit")]
        public IActionResult Edit(int id)
        {
            var model = _playerRepository.GetFormById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /players/edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost("uredi/{id:int}")]
        [HttpPost("~/players/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PlayerFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            ValidatePlayerForm(model);

            if (!ModelState.IsValid)
            {
                _playerRepository.PopulateFormOptions(model);
                return View(model);
            }

            var updated = _playerRepository.Update(id, model);

            if (!updated)
                return NotFound();

            _logger.LogInformation("Player {PlayerId} updated by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /players/delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("obrisi/{id:int}")]
        [HttpGet("~/players/delete/{id:int}", Name = "player-delete")]
        public IActionResult Delete(int id)
        {
            var model = _playerRepository.GetById(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST /players/delete/{id}
        [HttpPost("obrisi/{id:int}")]
        [Authorize(Roles = "Admin")]
        [HttpPost("~/players/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public IActionResult DeleteConfirmed(int id)
        {
            var deleted = _playerRepository.Delete(id);

            if (!deleted)
                return NotFound();

            _logger.LogInformation("Player {PlayerId} deleted by {User}.", id, User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        // ── Image upload (Dropzone) ──────────────────────────────────────────

        // POST /players/{id}/images — Dropzone posts one file per request as "file".
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/slike")]
        [HttpPost("~/players/{id:int}/images")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var (image, error) = await _imageRepository.AddAsync(ImageOwnerType.Player, id, file);

            if (error != null)
                return BadRequest(error);

            _logger.LogInformation("Image {ImageId} uploaded to player {PlayerId} by {User}.", image!.Id, id, User.Identity?.Name);
            return Json(new { success = true, id = image!.Id });
        }

        // GET /players/{id}/images — AJAX: renders the gallery partial.
        [HttpGet("{id:int}/slike")]
        [HttpGet("~/players/{id:int}/images")]
        [AllowAnonymous]
        public IActionResult GetImages(int id)
        {
            ViewData["PrimaryNoun"] = "photo";
            return PartialView("_ImageList", _imageRepository.GetForOwner(ImageOwnerType.Player, id));
        }

        // POST /players/images/delete — imageId comes from the AJAX request body.
        [Authorize(Roles = "Admin")]
        [HttpPost("slike/obrisi")]
        [HttpPost("~/players/images/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int imageId)
        {
            if (!_imageRepository.Delete(imageId))
                return NotFound();

            _logger.LogInformation("Player image {ImageId} deleted by {User}.", imageId, User.Identity?.Name);
            return Json(new { success = true });
        }

        // POST /players/images/primary — mark as the player headshot.
        [Authorize(Roles = "Admin")]
        [HttpPost("slike/glavna")]
        [HttpPost("~/players/images/primary")]
        [ValidateAntiForgeryToken]
        public IActionResult SetPrimaryImage(int imageId)
        {
            return _imageRepository.SetPrimary(imageId) ? Json(new { success = true }) : NotFound();
        }

        // Checks that the selected club actually exists in the database.
        private void ValidatePlayerForm(PlayerFormViewModel model)
        {
            if (model.ClubId.HasValue && !_playerRepository.ClubExists(model.ClubId.Value))
                ModelState.AddModelError(nameof(model.ClubId), "The selected club does not exist.");
        }
    }
}
