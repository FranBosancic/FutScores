using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class RatingController : Controller
    {
        private readonly RatingMockRepository _ratingRepository;
        private readonly PlayerMockRepository _playerRepository;
        private readonly MatchMockRepository _matchRepository;
        private readonly UserMockRepository _userRepository;
        private readonly ClubMockRepository _clubRepository;

        public RatingController(
            RatingMockRepository ratingRepository,
            PlayerMockRepository playerRepository,
            MatchMockRepository matchRepository,
            UserMockRepository userRepository,
            ClubMockRepository clubRepository)
        {
            _ratingRepository = ratingRepository;
            _playerRepository = playerRepository;
            _matchRepository = matchRepository;
            _userRepository = userRepository;
            _clubRepository = clubRepository;
        }

        public IActionResult Index()
        {
            var ratings = _ratingRepository.GetAll();

            var ratingViewModels = ratings.Select(rating =>
            {
                var player = _playerRepository.GetById(rating.PlayerId);
                var match = _matchRepository.GetById(rating.MatchId);
                var user = _userRepository.GetById(rating.UserId);
                var homeClub = match != null ? _clubRepository.GetById(match.HomeTeamId) : null;
                var awayClub = match != null ? _clubRepository.GetById(match.AwayTeamId) : null;

                string matchDescription = "Unknown match";
                if (match != null)
                {
                    matchDescription = $"{homeClub?.Name ?? "Home"} vs {awayClub?.Name ?? "Away"} on {match.Date:yyyy-MM-dd}";
                }

                return new RatingDetailsViewModel
                {
                    Id = rating.Id,
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
                    UserId = rating.UserId,
                    PlayerName = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                    MatchDescription = matchDescription,
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User",
                    Score = rating.Score,
                    Comment = rating.Comment
                };
            }).ToList();

            return View(ratingViewModels);
        }

        public IActionResult Details(int id)
        {
            var rating = _ratingRepository.GetById(id);
            if (rating == null)
            {
                return NotFound();
            }

            var player = _playerRepository.GetById(rating.PlayerId);
            var match = _matchRepository.GetById(rating.MatchId);
            var user = _userRepository.GetById(rating.UserId);

            string matchDescription = "Unknown match";
            if (match != null)
            {
                var homeClub = _clubRepository.GetById(match.HomeTeamId);
                var awayClub = _clubRepository.GetById(match.AwayTeamId);
                matchDescription = $"{homeClub?.Name ?? "Home"} vs {awayClub?.Name ?? "Away"} on {match.Date:yyyy-MM-dd}";
            }

            var viewModel = new RatingDetailsViewModel
            {
                Id = rating.Id,
                PlayerId = rating.PlayerId,
                MatchId = rating.MatchId,
                UserId = rating.UserId,
                PlayerName = player != null ? $"{player.FirstName} {player.LastName}" : "Unknown Player",
                MatchDescription = matchDescription,
                UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown User",
                Score = rating.Score,
                Comment = rating.Comment
            };

            return View(viewModel);
        }
    }
}
