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
            return _dbContext.Leagues
                .AsNoTracking()
                .Where(league => league.Id == id)
                .Select(league => new LeagueDetailsViewModel
                {
                    Id = league.Id,
                    Name = league.Name,
                    ClubCount = league.Clubs.Count,
                    MatchCount = league.Matches.Count
                })
                .FirstOrDefault();
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