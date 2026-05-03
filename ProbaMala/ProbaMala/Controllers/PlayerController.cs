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
        public IActionResult Index()
        {
            return View(_playerRepository.GetAll());
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
    }
}
