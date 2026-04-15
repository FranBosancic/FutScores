using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class MatchMockRepository
    {
        private static readonly List<Match> _matches = new()
        {
            new Match { Id = 1, LeagueId = 1, HomeTeamId = 1, AwayTeamId = 2, Date = new DateTime(2024, 4, 15), HomeGoals = 2, AwayGoals = 1 },
            new Match { Id = 2, LeagueId = 1, HomeTeamId = 3, AwayTeamId = 1, Date = new DateTime(2024, 4, 20), HomeGoals = 1, AwayGoals = 1 },
            new Match { Id = 3, LeagueId = 1, HomeTeamId = 2, AwayTeamId = 3, Date = new DateTime(2024, 4, 25), HomeGoals = 3, AwayGoals = 2 },
            new Match { Id = 4, LeagueId = 2, HomeTeamId = 4, AwayTeamId = 5, Date = new DateTime(2024, 4, 18), HomeGoals = 2, AwayGoals = 2 },
            new Match { Id = 5, LeagueId = 2, HomeTeamId = 5, AwayTeamId = 4, Date = new DateTime(2024, 5, 1), HomeGoals = 0, AwayGoals = 1 },
            new Match { Id = 6, LeagueId = 3, HomeTeamId = 6, AwayTeamId = 7, Date = new DateTime(2024, 4, 22), HomeGoals = 1, AwayGoals = 0 },
            new Match { Id = 7, LeagueId = 4, HomeTeamId = 8, AwayTeamId = 9, Date = new DateTime(2024, 4, 19), HomeGoals = 3, AwayGoals = 1 },
            new Match { Id = 8, LeagueId = 5, HomeTeamId = 10, AwayTeamId = 1, Date = new DateTime(2024, 4, 21), HomeGoals = 2, AwayGoals = 3 }
        };

        public List<Match> GetAll() => _matches;

        public Match? GetById(int id) => _matches.FirstOrDefault(m => m.Id == id);

        public List<Match> GetByLeagueId(int leagueId) => _matches.Where(m => m.LeagueId == leagueId).ToList();

        public List<Match> GetByClubId(int clubId) => _matches
            .Where(m => m.HomeTeamId == clubId || m.AwayTeamId == clubId)
            .ToList();
    }
}
