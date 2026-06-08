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
            var usersQuery = _dbContext.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.ToLower().Contains(q) ||
                    u.LastName.ToLower().Contains(q) ||
                    u.Email.ToLower().Contains(q));
            }

            return usersQuery
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserDetailsViewModel
                {
                    Id          = u.Id,
                    FirstName   = u.FirstName,
                    LastName    = u.LastName,
                    Email       = u.Email,
                    // Count inside EF query so it translates to SQL COUNT(*)
                    RatingCount = u.Ratings.Count
                })
                .ToList();
        }

        // Loads the user with their full rating history.
        // We use .AsEnumerable() + FirstOrDefault pattern (load first, then project)
        // because the nested RatingDetailsViewModel projection can't be translated to SQL.
        public UserDetailsViewModel? GetById(int id)
        {
            var user = _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Ratings).ThenInclude(r => r.Player)
                .Include(u => u.Ratings).ThenInclude(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(u => u.Ratings).ThenInclude(r => r.Match).ThenInclude(m => m.AwayTeam)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return null;

            return new UserDetailsViewModel
            {
                Id          = user.Id,
                FirstName   = user.FirstName,
                LastName    = user.LastName,
                Email       = user.Email,
                RatingCount = user.Ratings.Count,
                Ratings     = user.Ratings
                    .OrderByDescending(r => r.Match.Date)
                    .Select(r => new RatingDetailsViewModel
                    {
                        Id               = r.Id,
                        PlayerId         = r.PlayerId,
                        MatchId          = r.MatchId,
                        UserId           = r.UserId,
                        PlayerName       = $"{r.Player.FirstName} {r.Player.LastName}",
                        MatchDescription = $"{r.Match.HomeTeam.Name} vs {r.Match.AwayTeam.Name} on {r.Match.Date:yyyy-MM-dd}",
                        UserName         = $"{user.FirstName} {user.LastName}",
                        Score            = r.Score,
                        Comment          = r.Comment
                    })
                    .ToList()
            };
        }

        public UserFormViewModel BuildFormModel()
        {
            return new UserFormViewModel();
        }

        public UserFormViewModel? GetFormById(int id)
        {
            return _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserFormViewModel
                {
                    Id        = u.Id,
                    FirstName = u.FirstName,
                    LastName  = u.LastName,
                    Email     = u.Email
                })
                .FirstOrDefault();
        }

        // Case-insensitive uniqueness check; excludeUserId prevents flagging the
        // record's own email during an edit.
        public bool EmailExists(string email, int? excludeUserId = null)
        {
            var normalized = email.Trim().ToLower();

            return _dbContext.Users.Any(u =>
                u.Email.ToLower() == normalized &&
                (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        public int Add(UserFormViewModel model)
        {
            var entity = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName  = model.LastName.Trim(),
                Email     = model.Email.Trim()
            };

            _dbContext.Users.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, UserFormViewModel model)
        {
            var entity = _dbContext.Users.FirstOrDefault(u => u.Id == id);

            if (entity == null)
                return false;

            entity.FirstName = model.FirstName.Trim();
            entity.LastName  = model.LastName.Trim();
            entity.Email     = model.Email.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Users.FirstOrDefault(u => u.Id == id);

            if (entity == null)
                return false;

            _dbContext.Users.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
