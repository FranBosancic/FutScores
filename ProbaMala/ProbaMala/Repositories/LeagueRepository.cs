using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;

namespace ProbaMala.Repositories
{
    public interface ILeagueRepository
    {
        List<League> GetAll();
        League? GetById(int id);
    }

    public class LeagueRepository : ILeagueRepository
    {
        private readonly AppDbContext _dbContext;

        public LeagueRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<League> GetAll()
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(league => league.Name)
                .ToList();
        }

        public League? GetById(int id)
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .FirstOrDefault(league => league.Id == id);
        }
    }
}