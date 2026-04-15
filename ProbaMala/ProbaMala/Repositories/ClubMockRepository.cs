using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class ClubMockRepository
    {
        private static readonly List<Club> _clubs = new()
        {
            new Club { Id = 1, Name = "Manchester United", FoundedDate = new DateTime(1878, 1, 1), LeagueId = 1 },
            new Club { Id = 2, Name = "Liverpool FC", FoundedDate = new DateTime(1892, 1, 1), LeagueId = 1 },
            new Club { Id = 3, Name = "Manchester City", FoundedDate = new DateTime(1880, 1, 1), LeagueId = 1 },
            new Club { Id = 4, Name = "Real Madrid", FoundedDate = new DateTime(1902, 1, 1), LeagueId = 2 },
            new Club { Id = 5, Name = "FC Barcelona", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 2 },
            new Club { Id = 6, Name = "AC Milan", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 3 },
            new Club { Id = 7, Name = "Juventus", FoundedDate = new DateTime(1897, 1, 1), LeagueId = 3 },
            new Club { Id = 8, Name = "Bayern Munich", FoundedDate = new DateTime(1900, 1, 1), LeagueId = 4 },
            new Club { Id = 9, Name = "Borussia Dortmund", FoundedDate = new DateTime(1909, 1, 1), LeagueId = 4 },
            new Club { Id = 10, Name = "Paris Saint-Germain", FoundedDate = new DateTime(1970, 1, 1), LeagueId = 5 }
        };

        public List<Club> GetAll() => _clubs;

        public Club? GetById(int id) => _clubs.FirstOrDefault(c => c.Id == id);

        public List<Club> GetByLeagueId(int leagueId) => _clubs.Where(c => c.LeagueId == leagueId).ToList();
    }
}
