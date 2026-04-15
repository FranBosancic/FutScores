using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class LeagueMockRepository
    {
        private static readonly List<League> _leagues = new()
        {
            new League { Id = 1, Name = "Premier League" },
            new League { Id = 2, Name = "La Liga" },
            new League { Id = 3, Name = "Serie A" },
            new League { Id = 4, Name = "Bundesliga" },
            new League { Id = 5, Name = "Ligue 1" }
        };

        public List<League> GetAll() => _leagues;

        public League? GetById(int id) => _leagues.FirstOrDefault(l => l.Id == id);
    }
}
