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

            var playerViewModels = players.Select(player =>
            {
                var club = _clubRepository.GetById(player.ClubId);

                return new PlayerDetailsViewModel
                {
                    Id = player.Id,
                    ClubId = player.ClubId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    Nationality = player.Nationality,
                    ClubName = club?.Name ?? "Unknown"
                };
            }).ToList();

            return View(playerViewModels);
        }

        public IActionResult Details(int id)
        {
            var player = _playerRepository.GetById(id);
            if (player == null)
            {
                return NotFound();
            }

            var club = _clubRepository.GetById(player.ClubId);
            var viewModel = new PlayerDetailsViewModel
            {
                Id = player.Id,
                ClubId = player.ClubId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DateOfBirth = player.DateOfBirth,
                Position = player.Position,
                Nationality = player.Nationality,
                ClubName = club?.Name ?? "Unknown"
            };

            return View(viewModel);
        }
    }
}
