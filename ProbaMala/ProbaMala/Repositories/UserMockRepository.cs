using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class UserMockRepository
    {
        private static readonly List<User> _users = new()
        {
            new User { Id = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@email.com" },
            new User { Id = 2, FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@email.com" },
            new User { Id = 3, FirstName = "Alex", LastName = "Johnson", Email = "alex.johnson@email.com" },
            new User { Id = 4, FirstName = "Sofia", LastName = "Rossi", Email = "sofia.rossi@email.com" },
            new User { Id = 5, FirstName = "Lucas", LastName = "Silva", Email = "lucas.silva@email.com" }
        };

        public List<User> GetAll() => _users;

        public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);
    }
}
