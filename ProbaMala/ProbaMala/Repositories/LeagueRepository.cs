using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface ILeagueRepository
    {
        List<LeagueDetailsViewModel> GetAll(string? query = null);
        LeagueDetailsViewModel? GetById(int id);
        LeagueFormViewModel BuildFormModel();
        LeagueFormViewModel? GetFormById(int id);
        bool NameExists(string name, int? excludeLeagueId = null);
        int Add(LeagueFormViewModel model);
        bool Update(int id, LeagueFormViewModel model);
        bool Delete(int id);
    }

    public class LeagueRepository : ILeagueRepository
    {
        private readonly AppDbContext _dbContext;

        public LeagueRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<LeagueDetailsViewModel> GetAll(string? query = null)
        {
            var leaguesQuery = _dbContext.Leagues
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                leaguesQuery = leaguesQuery.Where(league => league.Name.ToLower().Contains(normalizedQuery));
            }

            return leaguesQuery
                .OrderBy(league => league.Name)
                .Select(league => new LeagueDetailsViewModel
                {
                    Id = league.Id,
                    Name = league.Name,
                    ClubCount = league.Clubs.Count,
                    MatchCount = league.Matches.Count
                })
                .ToList();
        }

        public LeagueDetailsViewModel? GetById(int id)
        {
            // Load the league with its clubs and matches in a single query via Include.
            // We use AsEnumerable() to do the final shaping in-memory since EF can't
            // translate the MatchDetailsViewModel's status logic to SQL.
            var league = _dbContext.Leagues
                .AsNoTracking()
                .Include(l => l.Clubs)
                    .ThenInclude(c => c.Players)
                .Include(l => l.Clubs)
                    .ThenInclude(c => c.HomeMatches)
                .Include(l => l.Clubs)
                    .ThenInclude(c => c.AwayMatches)
                .Include(l => l.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(l => l.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .Include(l => l.Matches)
                    .ThenInclude(m => m.Ratings)
                .FirstOrDefault(l => l.Id == id);

            if (league == null) return null;

            return new LeagueDetailsViewModel
            {
                Id = league.Id,
                Name = league.Name,
                ClubCount = league.Clubs.Count,
                MatchCount = league.Matches.Count,

                // Clubs alphabetically with key stats
                Clubs = league.Clubs
                    .OrderBy(c => c.Name)
                    .Select(c => new ClubDetailsViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        LeagueId = league.Id,
                        LeagueName = league.Name,
                        FoundedDate = c.FoundedDate,
                        PlayerCount = c.Players.Count,
                        MatchCount = c.HomeMatches.Count + c.AwayMatches.Count
                    })
                    .ToList(),

                // Matches newest-first; past = Finished, future = Scheduled
                Matches = league.Matches
                    .OrderByDescending(m => m.Date)
                    .Select(m => new MatchDetailsViewModel
                    {
                        Id = m.Id,
                        LeagueId = league.Id,
                        HomeTeamId = m.HomeTeamId,
                        AwayTeamId = m.AwayTeamId,
                        Date = m.Date,
                        KickoffLabel = m.Date.ToString("dd MMM yyyy"),
                        StatusLabel = m.Date <= DateTime.UtcNow ? "Finished" : "Scheduled",
                        StatusTone  = m.Date <= DateTime.UtcNow ? "final" : "upcoming",
                        LeagueName = league.Name,
                        HomeTeamName = m.HomeTeam.Name,
                        AwayTeamName = m.AwayTeam.Name,
                        HomeGoals = m.HomeGoals,
                        AwayGoals = m.AwayGoals,
                        RatingCount = m.Ratings.Count
                    })
                    .ToList()
            };
        }

        public LeagueFormViewModel BuildFormModel()
        {
            return new LeagueFormViewModel();
        }

        public LeagueFormViewModel? GetFormById(int id)
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .Where(league => league.Id == id)
                .Select(league => new LeagueFormViewModel
                {
                    Id = league.Id,
                    Name = league.Name
                })
                .FirstOrDefault();
        }

        public bool NameExists(string name, int? excludeLeagueId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return _dbContext.Leagues.Any(league =>
                league.Name.ToLower() == normalizedName &&
                (!excludeLeagueId.HasValue || league.Id != excludeLeagueId.Value));
        }

        public int Add(LeagueFormViewModel model)
        {
            var entity = new League
            {
                Name = model.Name.Trim()
            };

            _dbContext.Leagues.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, LeagueFormViewModel model)
        {
            var entity = _dbContext.Leagues.FirstOrDefault(league => league.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.Name = model.Name.Trim();
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Leagues.FirstOrDefault(league => league.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Leagues.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}