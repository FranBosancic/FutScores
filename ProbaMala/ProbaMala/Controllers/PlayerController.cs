using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class PlayerController : Controller
    {
        private readonly PlayerMockRepository _playerRepository;
        private readonly ClubMockRepository _clubRepository;

        public PlayerController(PlayerMockRepository playerRepository, ClubMockRepository clubRepository)
        {
            _playerRepository = playerRepository;
            _clubRepository = clubRepository;
        }

        public IActionResult Index()
        {
            var players = _playerRepository.GetAll();
            return View(players);
        }

        public IActionResult Details(int id)
        {
            var player = _playerRepository.GetById(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }
    }
}
