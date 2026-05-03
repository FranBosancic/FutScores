using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IPlayerRepository
    {
        List<PlayerDetailsViewModel> GetAll();
        PlayerDetailsViewModel? GetById(int id);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PlayerDetailsViewModel> GetAll()
        {
            return _dbContext.Players
                .AsNoTracking()
                .Include(player => player.Club)
                .OrderBy(player => player.LastName)
                .ThenBy(player => player.FirstName)
                .Select(player => new PlayerDetailsViewModel
                {
                    Id = player.Id,
                    ClubId = player.ClubId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    Nationality = player.Nationality,
                    ClubName = player.Club.Name
                })
                .ToList();
        }

        public PlayerDetailsViewModel? GetById(int id)
        {
            return _dbContext.Players
                .AsNoTracking()
                .Include(player => player.Club)
                .Where(player => player.Id == id)
                .Select(player => new PlayerDetailsViewModel
                {
                    Id = player.Id,
                    ClubId = player.ClubId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    Nationality = player.Nationality,
                    ClubName = player.Club.Name
                })
                .FirstOrDefault();
        }
    }
}