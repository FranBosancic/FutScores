using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;

namespace ProbaMala.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetAll()
        {
            return _dbContext.Users
                .AsNoTracking()
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .ToList();
        }

        public User? GetById(int id)
        {
            return _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Id == id);
        }
    }
}