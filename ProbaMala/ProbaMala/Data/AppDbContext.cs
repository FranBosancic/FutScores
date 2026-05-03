using Microsoft.EntityFrameworkCore;
using ProbaMala.Models.Entities;

namespace ProbaMala.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<League> Leagues => Set<League>();
        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Rating> Ratings => Set<Rating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Club>()
                .Property(club => club.FoundedDate)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Player>()
                .Property(player => player.DateOfBirth)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Match>()
                .Property(match => match.Date)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Match>()
                .HasOne(match => match.HomeTeam)
                .WithMany(club => club.HomeMatches)
                .HasForeignKey(match => match.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(match => match.AwayTeam)
                .WithMany(club => club.AwayMatches)
                .HasForeignKey(match => match.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<League>().HasData(
                new League { Id = 1, Name = "Premier League" },
                new League { Id = 2, Name = "La Liga" },
                new League { Id = 3, Name = "Serie A" },
                new League { Id = 4, Name = "Bundesliga" },
                new League { Id = 5, Name = "Ligue 1" });

            modelBuilder.Entity<Club>().HasData(
                new Club { Id = 1, Name = "Manchester United", FoundedDate = new DateTime(1878, 1, 1), LeagueId = 1 },
                new Club { Id = 2, Name = "Liverpool FC", FoundedDate = new DateTime(1892, 1, 1), LeagueId = 1 },
                new Club { Id = 3, Name = "Manchester City", FoundedDate = new DateTime(1880, 1, 1), LeagueId = 1 },
                new Club { Id = 4, Name = "Real Madrid", FoundedDate = new DateTime(1902, 1, 1), LeagueId = 2 },
                new Club { Id = 5, Name = "FC Barcelona", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 2 },
                new Club { Id = 6, Name = "AC Milan", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 3 },
                new Club { Id = 7, Name = "Juventus", FoundedDate = new DateTime(1897, 1, 1), LeagueId = 3 },
                new Club { Id = 8, Name = "Bayern Munich", FoundedDate = new DateTime(1900, 1, 1), LeagueId = 4 },
                new Club { Id = 9, Name = "Borussia Dortmund", FoundedDate = new DateTime(1909, 1, 1), LeagueId = 4 },
                new Club { Id = 10, Name = "Paris Saint-Germain", FoundedDate = new DateTime(1970, 1, 1), LeagueId = 5 });

            modelBuilder.Entity<Player>().HasData(
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
                new Player { Id = 15, FirstName = "Kylian", LastName = "Mbappe", DateOfBirth = new DateTime(1998, 12, 20), Position = Position.Forward, ClubId = 10, Nationality = "France" });

            modelBuilder.Entity<Match>().HasData(
                new Match { Id = 1, LeagueId = 1, HomeTeamId = 1, AwayTeamId = 2, Date = new DateTime(2024, 4, 15), HomeGoals = 2, AwayGoals = 1 },
                new Match { Id = 2, LeagueId = 1, HomeTeamId = 3, AwayTeamId = 1, Date = new DateTime(2024, 4, 20), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 3, LeagueId = 1, HomeTeamId = 2, AwayTeamId = 3, Date = new DateTime(2024, 4, 25), HomeGoals = 3, AwayGoals = 2 },
                new Match { Id = 4, LeagueId = 2, HomeTeamId = 4, AwayTeamId = 5, Date = new DateTime(2024, 4, 18), HomeGoals = 2, AwayGoals = 2 },
                new Match { Id = 5, LeagueId = 2, HomeTeamId = 5, AwayTeamId = 4, Date = new DateTime(2024, 5, 1), HomeGoals = 0, AwayGoals = 1 },
                new Match { Id = 6, LeagueId = 3, HomeTeamId = 6, AwayTeamId = 7, Date = new DateTime(2024, 4, 22), HomeGoals = 1, AwayGoals = 0 },
                new Match { Id = 7, LeagueId = 4, HomeTeamId = 8, AwayTeamId = 9, Date = new DateTime(2024, 4, 19), HomeGoals = 3, AwayGoals = 1 },
                new Match { Id = 8, LeagueId = 5, HomeTeamId = 10, AwayTeamId = 1, Date = new DateTime(2024, 4, 21), HomeGoals = 2, AwayGoals = 3 });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@email.com" },
                new User { Id = 2, FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@email.com" },
                new User { Id = 3, FirstName = "Alex", LastName = "Johnson", Email = "alex.johnson@email.com" },
                new User { Id = 4, FirstName = "Sofia", LastName = "Rossi", Email = "sofia.rossi@email.com" },
                new User { Id = 5, FirstName = "Lucas", LastName = "Silva", Email = "lucas.silva@email.com" });

            modelBuilder.Entity<Rating>().HasData(
                new Rating { Id = 1, PlayerId = 1, MatchId = 1, UserId = 1, Score = 9, Comment = "Excellent performance!" },
                new Rating { Id = 2, PlayerId = 2, MatchId = 1, UserId = 2, Score = 7, Comment = "Solid defense" },
                new Rating { Id = 3, PlayerId = 4, MatchId = 3, UserId = 1, Score = 8, Comment = "Great goal!" },
                new Rating { Id = 4, PlayerId = 5, MatchId = 3, UserId = 3, Score = 7, Comment = "Defensive masterclass" },
                new Rating { Id = 5, PlayerId = 6, MatchId = 2, UserId = 2, Score = 9, Comment = "Best striker in the league" },
                new Rating { Id = 6, PlayerId = 7, MatchId = 2, UserId = 4, Score = 8, Comment = "Great midfield control" },
                new Rating { Id = 7, PlayerId = 11, MatchId = 7, UserId = 5, Score = 9, Comment = "Clinical finishing" },
                new Rating { Id = 8, PlayerId = 12, MatchId = 7, UserId = 1, Score = 8, Comment = "Impressive technical skills" });
        }
    }
}