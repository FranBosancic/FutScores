using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IClubRepository
    {
        List<ClubDetailsViewModel> GetAll();
        ClubDetailsViewModel? GetById(int id);
    }

    public class ClubRepository : IClubRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ClubDetailsViewModel> GetAll()
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Include(club => club.League)
                .OrderBy(club => club.Name)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name
                })
                .ToList();
        }

        public ClubDetailsViewModel? GetById(int id)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Include(club => club.League)
                .Where(club => club.Id == id)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name
                })
                .FirstOrDefault();
        }
    }
}