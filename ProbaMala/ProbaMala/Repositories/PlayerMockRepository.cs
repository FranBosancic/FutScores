using ProbaMala.Models;

namespace ProbaMala.Repositories
{
    public class PlayerMockRepository
    {
        private static readonly List<Player> _players = new()
        {
            new Player { Id = 1, FirstName = "Cristiano", LastName = "Ronaldo", DateOfBirth = new DateTime(1985, 2, 5), Position = Position.Forward, ClubId = 1, Nationality = "Portugal" },
            new Player { Id = 2, FirstName = "Harry", LastName = "Maguire", DateOfBirth = new DateTime(1993, 3, 5), Position = Position.Defender, ClubId = 1, Nationality = "England" },
            new Player { Id = 3, FirstName = "David", LastName = "De Gea", DateOfBirth = new DateTime(1990, 11, 3), Position = Position.Goalkeeper, ClubId = 1, Nationality = "Spain" },
            new Player { Id = 4, FirstName = "Mohamed", LastName = "Salah", DateOfBirth = new DateTime(1992, 6, 15), Position = Position.Forward, ClubId = 2, Nationality = "Egypt" },
            new Player { Id = 5, FirstName = "Virgil", LastName = "van Dijk", DateOfBirth = new DateTime(1991, 7, 8), Position = Position.Defender, ClubId = 2, Nationality = "Netherlands" },
            new Player { Id = 6, FirstName = "Erling", LastName = "Haaland", DateOfBirth = new DateTime(2000, 7, 21), Position = Position.Forward, ClubId = 3, Nationality = "Norway" },
            new Player { Id = 7, FirstName = "Kevin", LastName = "De Bruyne", DateOfBirth = new DateTime(1991, 1, 28), Position = Position.Midfielder, ClubId = 3, Nationality = "Belgium" },
            new Player { Id = 8, FirstName = "Karim", LastName = "Benzema", DateOfBirth = new DateTime(1987, 12, 19), Position = Position.Forward, ClubId = 4, Nationality = "France" },
            new Player { Id = 9, FirstName = "Luka", LastName = "Modric", DateOfBirth = new DateTime(1985, 9, 9), Position = Position.Midfielder, ClubId = 4, Nationality = "Croatia" },
            new Player { Id = 10, FirstName = "Sergio", LastName = "Busquets", DateOfBirth = new DateTime(1988, 7, 16), Position = Position.Midfielder, ClubId = 5, Nationality = "Spain" },
            new Player { Id = 11, FirstName = "Robert", LastName = "Lewandowski", DateOfBirth = new DateTime(1988, 8, 21), Position = Position.Forward, ClubId = 8, Nationality = "Poland" },
            new Player { Id = 12, FirstName = "Jamal", LastName = "Musiala", DateOfBirth = new DateTime(2003, 2, 26), Position = Position.Midfielder, ClubId = 8, Nationality = "Germany" },
            new Player { Id = 13, FirstName = "Marco", LastName = "Reus", DateOfBirth = new DateTime(1990, 5, 31), Position = Position.Forward, ClubId = 9, Nationality = "Germany" },
            new Player { Id = 14, FirstName = "Thomas", LastName = "Tuchel", DateOfBirth = new DateTime(1989, 12, 1), Position = Position.Midfielder, ClubId = 10, Nationality = "France" },
            new Player { Id = 15, FirstName = "Kylian", LastName = "Mbappe", DateOfBirth = new DateTime(1998, 12, 20), Position = Position.Forward, ClubId = 10, Nationality = "France" }
        };

        public List<Player> GetAll() => _players;

        public Player? GetById(int id) => _players.FirstOrDefault(p => p.Id == id);

        public List<Player> GetByClubId(int clubId) => _players.Where(p => p.ClubId == clubId).ToList();
    }
}
