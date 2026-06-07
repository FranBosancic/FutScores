using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IUserRepository
    {
        List<UserDetailsViewModel> GetAll(string? query = null);
        UserDetailsViewModel? GetById(int id);
        UserFormViewModel BuildFormModel();
        UserFormViewModel? GetFormById(int id);
        bool EmailExists(string email, int? excludeUserId = null);
        int Add(UserFormViewModel model);
        bool Update(int id, UserFormViewModel model);
        bool Delete(int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UserDetailsViewModel> GetAll(string? query = null)
        {
            var usersQuery = _dbContext.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                usersQuery = usersQuery.Where(user =>
                    user.FirstName.ToLower().Contains(normalizedQuery) ||
                    user.LastName.ToLower().Contains(normalizedQuery) ||
                    user.Email.ToLower().Contains(normalizedQuery));
            }

            return usersQuery
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .Select(user => new UserDetailsViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RatingCount = user.Ratings.Count
                })
                .ToList();
        }

        public UserDetailsViewModel? GetById(int id)
        {
            return _dbContext.Users
                .AsNoTracking()
                .Include(user => user.Ratings).ThenInclude(rating => rating.Player)
                .Include(user => user.Ratings).ThenInclude(rating => rating.Match).ThenInclude(match => match.HomeTeam)
                .Include(user => user.Ratings).ThenInclude(rating => rating.Match).ThenInclude(match => match.AwayTeam)
                .Where(user => user.Id == id)
                .AsEnumerable()
                .Select(user => new UserDetailsViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RatingCount = user.Ratings.Count,
                    Ratings = user.Ratings
                        .OrderByDescending(rating => rating.Match.Date)
                        .Select(rating => new RatingDetailsViewModel
                        {
                            Id = rating.Id,
                            PlayerId = rating.PlayerId,
                            MatchId = rating.MatchId,
                            UserId = rating.UserId,
                            PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                            MatchDescription = $"{rating.Match.HomeTeam.Name} vs {rating.Match.AwayTeam.Name} on {rating.Match.Date:yyyy-MM-dd}",
                            UserName = $"{user.FirstName} {user.LastName}",
                            Score = rating.Score,
                            Comment = rating.Comment
                        })
                        .ToList()
                })
                .FirstOrDefault();
        }

        public UserFormViewModel BuildFormModel()
        {
            return new UserFormViewModel();
        }

        public UserFormViewModel? GetFormById(int id)
        {
            return _dbContext.Users
                .AsNoTracking()
                .Where(user => user.Id == id)
                .Select(user => new UserFormViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                })
                .FirstOrDefault();
        }

        public bool EmailExists(string email, int? excludeUserId = null)
        {
            var normalizedEmail = email.Trim().ToLower();

            return _dbContext.Users.Any(user =>
                user.Email.ToLower() == normalizedEmail &&
                (!excludeUserId.HasValue || user.Id != excludeUserId.Value));
        }

        public int Add(UserFormViewModel model)
        {
            var entity = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = model.Email.Trim()
            };

            _dbContext.Users.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, UserFormViewModel model)
        {
            var entity = _dbContext.Users.FirstOrDefault(user => user.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.FirstName = model.FirstName.Trim();
            entity.LastName = model.LastName.Trim();
            entity.Email = model.Email.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Users.FirstOrDefault(user => user.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Users.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}