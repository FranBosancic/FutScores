using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IPlayerRepository
    {
        List<PlayerDetailsViewModel> GetAll(string? query = null);
        PlayerDetailsViewModel? GetById(int id);
        PlayerFormViewModel BuildFormModel();
        PlayerFormViewModel? GetFormById(int id);
        void PopulateFormOptions(PlayerFormViewModel model);
        bool ClubExists(int clubId);
        int Add(PlayerFormViewModel model);
        bool Update(int id, PlayerFormViewModel model);
        bool Delete(int id);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PlayerDetailsViewModel> GetAll(string? query = null)
        {
            var playersQuery = _dbContext.Players
                .AsNoTracking()
                .Include(player => player.Club)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                playersQuery = playersQuery.Where(player =>
                    player.FirstName.ToLower().Contains(normalizedQuery) ||
                    player.LastName.ToLower().Contains(normalizedQuery) ||
                    player.Nationality.ToLower().Contains(normalizedQuery) ||
                    player.Club.Name.ToLower().Contains(normalizedQuery) ||
                    player.Position.ToString().ToLower().Contains(normalizedQuery));
            }

            return playersQuery
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
                    ClubName = player.Club.Name,
                    RatingCount = player.Ratings.Count
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
                    ClubName = player.Club.Name,
                    RatingCount = player.Ratings.Count
                })
                .FirstOrDefault();
        }

        public PlayerFormViewModel BuildFormModel()
        {
            var model = new PlayerFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public PlayerFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Players
                .AsNoTracking()
                .Where(player => player.Id == id)
                .Select(player => new PlayerFormViewModel
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    ClubId = player.ClubId,
                    Nationality = player.Nationality
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(PlayerFormViewModel model)
        {
            model.ClubOptions = _dbContext.Clubs
                .AsNoTracking()
                .OrderBy(club => club.Name)
                .Select(club => new SelectListItem
                {
                    Value = club.Id.ToString(),
                    Text = club.Name,
                    Selected = model.ClubId == club.Id
                })
                .ToList();

            model.PositionOptions = Enum.GetValues<Position>()
                .Select(position => new SelectListItem
                {
                    Value = position.ToString(),
                    Text = position.ToString(),
                    Selected = model.Position == position
                })
                .ToList();
        }

        public bool ClubExists(int clubId)
        {
            return _dbContext.Clubs.Any(club => club.Id == clubId);
        }

        public int Add(PlayerFormViewModel model)
        {
            var entity = new Player
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                DateOfBirth = model.DateOfBirth,
                Position = model.Position!.Value,
                ClubId = model.ClubId!.Value,
                Nationality = model.Nationality.Trim()
            };

            _dbContext.Players.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, PlayerFormViewModel model)
        {
            var entity = _dbContext.Players.FirstOrDefault(player => player.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.FirstName = model.FirstName.Trim();
            entity.LastName = model.LastName.Trim();
            entity.DateOfBirth = model.DateOfBirth;
            entity.Position = model.Position!.Value;
            entity.ClubId = model.ClubId!.Value;
            entity.Nationality = model.Nationality.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Players.FirstOrDefault(player => player.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Players.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}