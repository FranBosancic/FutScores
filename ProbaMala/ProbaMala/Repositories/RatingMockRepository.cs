using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class RatingMockRepository
    {
        private static readonly List<Rating> _ratings = new()
        {
            new Rating { Id = 1, PlayerId = 1, MatchId = 1, UserId = 1, Score = 9, Comment = "Excellent performance!" },
            new Rating { Id = 2, PlayerId = 2, MatchId = 1, UserId = 2, Score = 7, Comment = "Solid defense" },
            new Rating { Id = 3, PlayerId = 4, MatchId = 3, UserId = 1, Score = 8, Comment = "Great goal!" },
            new Rating { Id = 4, PlayerId = 5, MatchId = 3, UserId = 3, Score = 7, Comment = "Defensive masterclass" },
            new Rating { Id = 5, PlayerId = 6, MatchId = 2, UserId = 2, Score = 9, Comment = "Best striker in the league" },
            new Rating { Id = 6, PlayerId = 7, MatchId = 2, UserId = 4, Score = 8, Comment = "Great midfield control" },
            new Rating { Id = 7, PlayerId = 11, MatchId = 7, UserId = 5, Score = 9, Comment = "Clinical finishing" },
            new Rating { Id = 8, PlayerId = 12, MatchId = 7, UserId = 1, Score = 8, Comment = "Impressive technical skills" }
        };

        public List<Rating> GetAll() => _ratings;

        public Rating? GetById(int id) => _ratings.FirstOrDefault(r => r.Id == id);

        public List<Rating> GetByPlayerId(int playerId) => _ratings.Where(r => r.PlayerId == playerId).ToList();

        public List<Rating> GetByMatchId(int matchId) => _ratings.Where(r => r.MatchId == matchId).ToList();

        public List<Rating> GetByUserId(int userId) => _ratings.Where(r => r.UserId == userId).ToList();
    }
}
