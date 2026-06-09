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
        public DbSet<Image> Images => Set<Image>();

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

            // An Image belongs to exactly one owner. Both FKs are nullable; the
            // application sets exactly one. Deleting the owner cascades its images.
            modelBuilder.Entity<Image>()
                .Property(image => image.CreatedAt)
                .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<Image>()
                .HasOne(image => image.Club)
                .WithMany(club => club.Images)
                .HasForeignKey(image => image.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Image>()
                .HasOne(image => image.Player)
                .WithMany(player => player.Images)
                .HasForeignKey(image => image.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ─────────────────────────────────────────────────────────────────
            // Seed data: real 2025–26 season (Europe's top five leagues).
            // Clubs, squads and final standings verified against the 2025–26
            // Wikipedia season pages. Match scorelines are real results from the
            // same season; ratings/users are sample app data.
            // ─────────────────────────────────────────────────────────────────

            modelBuilder.Entity<League>().HasData(
                new League { Id = 1, Name = "Premier League" },
                new League { Id = 2, Name = "La Liga" },
                new League { Id = 3, Name = "Serie A" },
                new League { Id = 4, Name = "Bundesliga" },
                new League { Id = 5, Name = "Ligue 1" });

            modelBuilder.Entity<Club>().HasData(
                // Premier League
                new Club { Id = 1, Name = "Arsenal", FoundedDate = new DateTime(1886, 1, 1), LeagueId = 1 },
                new Club { Id = 2, Name = "Manchester City", FoundedDate = new DateTime(1880, 1, 1), LeagueId = 1 },
                new Club { Id = 3, Name = "Manchester United", FoundedDate = new DateTime(1878, 1, 1), LeagueId = 1 },
                new Club { Id = 4, Name = "Aston Villa", FoundedDate = new DateTime(1874, 1, 1), LeagueId = 1 },
                new Club { Id = 5, Name = "Liverpool", FoundedDate = new DateTime(1892, 1, 1), LeagueId = 1 },
                new Club { Id = 6, Name = "Chelsea", FoundedDate = new DateTime(1905, 1, 1), LeagueId = 1 },
                new Club { Id = 7, Name = "Newcastle United", FoundedDate = new DateTime(1892, 1, 1), LeagueId = 1 },
                new Club { Id = 8, Name = "Tottenham Hotspur", FoundedDate = new DateTime(1882, 1, 1), LeagueId = 1 },
                // La Liga
                new Club { Id = 9, Name = "FC Barcelona", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 2 },
                new Club { Id = 10, Name = "Real Madrid", FoundedDate = new DateTime(1902, 1, 1), LeagueId = 2 },
                new Club { Id = 11, Name = "Atlético Madrid", FoundedDate = new DateTime(1903, 1, 1), LeagueId = 2 },
                new Club { Id = 12, Name = "Villarreal", FoundedDate = new DateTime(1923, 1, 1), LeagueId = 2 },
                new Club { Id = 13, Name = "Real Betis", FoundedDate = new DateTime(1907, 1, 1), LeagueId = 2 },
                new Club { Id = 14, Name = "Athletic Bilbao", FoundedDate = new DateTime(1898, 1, 1), LeagueId = 2 },
                new Club { Id = 15, Name = "Real Sociedad", FoundedDate = new DateTime(1909, 1, 1), LeagueId = 2 },
                new Club { Id = 16, Name = "Sevilla", FoundedDate = new DateTime(1890, 1, 1), LeagueId = 2 },
                // Serie A
                new Club { Id = 17, Name = "Inter Milan", FoundedDate = new DateTime(1908, 1, 1), LeagueId = 3 },
                new Club { Id = 18, Name = "AC Milan", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 3 },
                new Club { Id = 19, Name = "Juventus", FoundedDate = new DateTime(1897, 1, 1), LeagueId = 3 },
                new Club { Id = 20, Name = "Napoli", FoundedDate = new DateTime(1926, 1, 1), LeagueId = 3 },
                new Club { Id = 21, Name = "Roma", FoundedDate = new DateTime(1927, 1, 1), LeagueId = 3 },
                new Club { Id = 22, Name = "Atalanta", FoundedDate = new DateTime(1907, 1, 1), LeagueId = 3 },
                new Club { Id = 23, Name = "Lazio", FoundedDate = new DateTime(1900, 1, 1), LeagueId = 3 },
                new Club { Id = 24, Name = "Fiorentina", FoundedDate = new DateTime(1926, 1, 1), LeagueId = 3 },
                // Bundesliga
                new Club { Id = 25, Name = "Bayern Munich", FoundedDate = new DateTime(1900, 1, 1), LeagueId = 4 },
                new Club { Id = 26, Name = "Borussia Dortmund", FoundedDate = new DateTime(1909, 1, 1), LeagueId = 4 },
                new Club { Id = 27, Name = "RB Leipzig", FoundedDate = new DateTime(2009, 1, 1), LeagueId = 4 },
                new Club { Id = 28, Name = "Bayer Leverkusen", FoundedDate = new DateTime(1904, 1, 1), LeagueId = 4 },
                new Club { Id = 29, Name = "VfB Stuttgart", FoundedDate = new DateTime(1893, 1, 1), LeagueId = 4 },
                new Club { Id = 30, Name = "Eintracht Frankfurt", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 4 },
                new Club { Id = 31, Name = "Borussia Mönchengladbach", FoundedDate = new DateTime(1900, 1, 1), LeagueId = 4 },
                // Ligue 1
                new Club { Id = 32, Name = "Paris Saint-Germain", FoundedDate = new DateTime(1970, 1, 1), LeagueId = 5 },
                new Club { Id = 33, Name = "Marseille", FoundedDate = new DateTime(1899, 1, 1), LeagueId = 5 },
                new Club { Id = 34, Name = "Monaco", FoundedDate = new DateTime(1924, 1, 1), LeagueId = 5 },
                new Club { Id = 35, Name = "Lyon", FoundedDate = new DateTime(1950, 1, 1), LeagueId = 5 },
                new Club { Id = 36, Name = "Lille", FoundedDate = new DateTime(1944, 1, 1), LeagueId = 5 },
                new Club { Id = 37, Name = "Lens", FoundedDate = new DateTime(1906, 1, 1), LeagueId = 5 });

            modelBuilder.Entity<Player>().HasData(
                // ── Arsenal (1) ──
                new Player { Id = 1, FirstName = "David", LastName = "Raya", DateOfBirth = new DateTime(1995, 9, 15), Position = Position.Goalkeeper, ClubId = 1, Nationality = "Spain" },
                new Player { Id = 2, FirstName = "William", LastName = "Saliba", DateOfBirth = new DateTime(2001, 3, 24), Position = Position.Defender, ClubId = 1, Nationality = "France" },
                new Player { Id = 3, FirstName = "Gabriel", LastName = "Magalhães", DateOfBirth = new DateTime(1997, 12, 19), Position = Position.Defender, ClubId = 1, Nationality = "Brazil" },
                new Player { Id = 4, FirstName = "Declan", LastName = "Rice", DateOfBirth = new DateTime(1999, 1, 14), Position = Position.Midfielder, ClubId = 1, Nationality = "England" },
                new Player { Id = 5, FirstName = "Martin", LastName = "Ødegaard", DateOfBirth = new DateTime(1998, 12, 17), Position = Position.Midfielder, ClubId = 1, Nationality = "Norway" },
                new Player { Id = 6, FirstName = "Martín", LastName = "Zubimendi", DateOfBirth = new DateTime(1999, 2, 2), Position = Position.Midfielder, ClubId = 1, Nationality = "Spain" },
                new Player { Id = 7, FirstName = "Bukayo", LastName = "Saka", DateOfBirth = new DateTime(2001, 9, 5), Position = Position.Forward, ClubId = 1, Nationality = "England" },
                new Player { Id = 8, FirstName = "Gabriel", LastName = "Martinelli", DateOfBirth = new DateTime(2001, 6, 18), Position = Position.Forward, ClubId = 1, Nationality = "Brazil" },
                new Player { Id = 9, FirstName = "Viktor", LastName = "Gyökeres", DateOfBirth = new DateTime(1998, 6, 4), Position = Position.Forward, ClubId = 1, Nationality = "Sweden" },
                new Player { Id = 10, FirstName = "Kai", LastName = "Havertz", DateOfBirth = new DateTime(1999, 6, 11), Position = Position.Forward, ClubId = 1, Nationality = "Germany" },
                // ── Manchester City (2) ──
                new Player { Id = 11, FirstName = "Gianluigi", LastName = "Donnarumma", DateOfBirth = new DateTime(1999, 2, 25), Position = Position.Goalkeeper, ClubId = 2, Nationality = "Italy" },
                new Player { Id = 12, FirstName = "Rúben", LastName = "Dias", DateOfBirth = new DateTime(1997, 5, 14), Position = Position.Defender, ClubId = 2, Nationality = "Portugal" },
                new Player { Id = 13, FirstName = "Joško", LastName = "Gvardiol", DateOfBirth = new DateTime(2002, 1, 23), Position = Position.Defender, ClubId = 2, Nationality = "Croatia" },
                new Player { Id = 14, FirstName = "Bernardo", LastName = "Silva", DateOfBirth = new DateTime(1994, 8, 10), Position = Position.Midfielder, ClubId = 2, Nationality = "Portugal" },
                new Player { Id = 15, FirstName = "Phil", LastName = "Foden", DateOfBirth = new DateTime(2000, 5, 28), Position = Position.Midfielder, ClubId = 2, Nationality = "England" },
                new Player { Id = 16, FirstName = "Tijjani", LastName = "Reijnders", DateOfBirth = new DateTime(1998, 7, 29), Position = Position.Midfielder, ClubId = 2, Nationality = "Netherlands" },
                new Player { Id = 17, FirstName = "Rayan", LastName = "Cherki", DateOfBirth = new DateTime(2003, 8, 17), Position = Position.Midfielder, ClubId = 2, Nationality = "France" },
                new Player { Id = 18, FirstName = "Erling", LastName = "Haaland", DateOfBirth = new DateTime(2000, 7, 21), Position = Position.Forward, ClubId = 2, Nationality = "Norway" },
                new Player { Id = 19, FirstName = "Jérémy", LastName = "Doku", DateOfBirth = new DateTime(2002, 5, 27), Position = Position.Forward, ClubId = 2, Nationality = "Belgium" },
                // ── Manchester United (3) ──
                new Player { Id = 20, FirstName = "André", LastName = "Onana", DateOfBirth = new DateTime(1996, 4, 2), Position = Position.Goalkeeper, ClubId = 3, Nationality = "Cameroon" },
                new Player { Id = 21, FirstName = "Matthijs", LastName = "de Ligt", DateOfBirth = new DateTime(1999, 8, 12), Position = Position.Defender, ClubId = 3, Nationality = "Netherlands" },
                new Player { Id = 22, FirstName = "Leny", LastName = "Yoro", DateOfBirth = new DateTime(2005, 11, 13), Position = Position.Defender, ClubId = 3, Nationality = "France" },
                new Player { Id = 23, FirstName = "Bruno", LastName = "Fernandes", DateOfBirth = new DateTime(1994, 9, 8), Position = Position.Midfielder, ClubId = 3, Nationality = "Portugal" },
                new Player { Id = 24, FirstName = "Kobbie", LastName = "Mainoo", DateOfBirth = new DateTime(2005, 4, 19), Position = Position.Midfielder, ClubId = 3, Nationality = "England" },
                new Player { Id = 25, FirstName = "Manuel", LastName = "Ugarte", DateOfBirth = new DateTime(2001, 4, 11), Position = Position.Midfielder, ClubId = 3, Nationality = "Uruguay" },
                new Player { Id = 26, FirstName = "Bryan", LastName = "Mbeumo", DateOfBirth = new DateTime(1999, 8, 7), Position = Position.Forward, ClubId = 3, Nationality = "Cameroon" },
                new Player { Id = 27, FirstName = "Matheus", LastName = "Cunha", DateOfBirth = new DateTime(1999, 5, 27), Position = Position.Forward, ClubId = 3, Nationality = "Brazil" },
                new Player { Id = 28, FirstName = "Benjamin", LastName = "Šeško", DateOfBirth = new DateTime(2003, 5, 31), Position = Position.Forward, ClubId = 3, Nationality = "Slovenia" },
                // ── Aston Villa (4) ──
                new Player { Id = 29, FirstName = "Emiliano", LastName = "Martínez", DateOfBirth = new DateTime(1992, 9, 2), Position = Position.Goalkeeper, ClubId = 4, Nationality = "Argentina" },
                new Player { Id = 30, FirstName = "Ezri", LastName = "Konsa", DateOfBirth = new DateTime(1997, 10, 23), Position = Position.Defender, ClubId = 4, Nationality = "England" },
                new Player { Id = 31, FirstName = "Pau", LastName = "Torres", DateOfBirth = new DateTime(1997, 1, 16), Position = Position.Defender, ClubId = 4, Nationality = "Spain" },
                new Player { Id = 32, FirstName = "Lucas", LastName = "Digne", DateOfBirth = new DateTime(1993, 7, 20), Position = Position.Defender, ClubId = 4, Nationality = "France" },
                new Player { Id = 33, FirstName = "John", LastName = "McGinn", DateOfBirth = new DateTime(1994, 10, 18), Position = Position.Midfielder, ClubId = 4, Nationality = "Scotland" },
                new Player { Id = 34, FirstName = "Youri", LastName = "Tielemans", DateOfBirth = new DateTime(1997, 5, 7), Position = Position.Midfielder, ClubId = 4, Nationality = "Belgium" },
                new Player { Id = 35, FirstName = "Morgan", LastName = "Rogers", DateOfBirth = new DateTime(2002, 7, 26), Position = Position.Midfielder, ClubId = 4, Nationality = "England" },
                new Player { Id = 36, FirstName = "Ollie", LastName = "Watkins", DateOfBirth = new DateTime(1995, 12, 30), Position = Position.Forward, ClubId = 4, Nationality = "England" },
                new Player { Id = 37, FirstName = "Emiliano", LastName = "Buendía", DateOfBirth = new DateTime(1996, 12, 25), Position = Position.Midfielder, ClubId = 4, Nationality = "Argentina" },
                // ── Liverpool (5) ──
                new Player { Id = 38, FirstName = "Alisson", LastName = "Becker", DateOfBirth = new DateTime(1992, 10, 2), Position = Position.Goalkeeper, ClubId = 5, Nationality = "Brazil" },
                new Player { Id = 39, FirstName = "Virgil", LastName = "van Dijk", DateOfBirth = new DateTime(1991, 7, 8), Position = Position.Defender, ClubId = 5, Nationality = "Netherlands" },
                new Player { Id = 40, FirstName = "Ibrahima", LastName = "Konaté", DateOfBirth = new DateTime(1999, 5, 25), Position = Position.Defender, ClubId = 5, Nationality = "France" },
                new Player { Id = 41, FirstName = "Milos", LastName = "Kerkez", DateOfBirth = new DateTime(2003, 11, 7), Position = Position.Defender, ClubId = 5, Nationality = "Hungary" },
                new Player { Id = 42, FirstName = "Ryan", LastName = "Gravenberch", DateOfBirth = new DateTime(2002, 5, 16), Position = Position.Midfielder, ClubId = 5, Nationality = "Netherlands" },
                new Player { Id = 43, FirstName = "Dominik", LastName = "Szoboszlai", DateOfBirth = new DateTime(2000, 10, 25), Position = Position.Midfielder, ClubId = 5, Nationality = "Hungary" },
                new Player { Id = 44, FirstName = "Alexis", LastName = "Mac Allister", DateOfBirth = new DateTime(1998, 12, 24), Position = Position.Midfielder, ClubId = 5, Nationality = "Argentina" },
                new Player { Id = 45, FirstName = "Florian", LastName = "Wirtz", DateOfBirth = new DateTime(2003, 5, 3), Position = Position.Midfielder, ClubId = 5, Nationality = "Germany" },
                new Player { Id = 46, FirstName = "Mohamed", LastName = "Salah", DateOfBirth = new DateTime(1992, 6, 15), Position = Position.Forward, ClubId = 5, Nationality = "Egypt" },
                new Player { Id = 47, FirstName = "Alexander", LastName = "Isak", DateOfBirth = new DateTime(1999, 9, 21), Position = Position.Forward, ClubId = 5, Nationality = "Sweden" },
                // ── Chelsea (6) ──
                new Player { Id = 48, FirstName = "Robert", LastName = "Sánchez", DateOfBirth = new DateTime(1997, 11, 18), Position = Position.Goalkeeper, ClubId = 6, Nationality = "Spain" },
                new Player { Id = 49, FirstName = "Levi", LastName = "Colwill", DateOfBirth = new DateTime(2003, 2, 26), Position = Position.Defender, ClubId = 6, Nationality = "England" },
                new Player { Id = 50, FirstName = "Reece", LastName = "James", DateOfBirth = new DateTime(1999, 12, 8), Position = Position.Defender, ClubId = 6, Nationality = "England" },
                new Player { Id = 51, FirstName = "Marc", LastName = "Cucurella", DateOfBirth = new DateTime(1998, 7, 22), Position = Position.Defender, ClubId = 6, Nationality = "Spain" },
                new Player { Id = 52, FirstName = "Moisés", LastName = "Caicedo", DateOfBirth = new DateTime(2001, 11, 2), Position = Position.Midfielder, ClubId = 6, Nationality = "Ecuador" },
                new Player { Id = 53, FirstName = "Enzo", LastName = "Fernández", DateOfBirth = new DateTime(2001, 1, 17), Position = Position.Midfielder, ClubId = 6, Nationality = "Argentina" },
                new Player { Id = 54, FirstName = "Cole", LastName = "Palmer", DateOfBirth = new DateTime(2002, 5, 6), Position = Position.Midfielder, ClubId = 6, Nationality = "England" },
                new Player { Id = 55, FirstName = "João", LastName = "Pedro", DateOfBirth = new DateTime(2001, 9, 26), Position = Position.Forward, ClubId = 6, Nationality = "Brazil" },
                new Player { Id = 56, FirstName = "Pedro", LastName = "Neto", DateOfBirth = new DateTime(2000, 3, 9), Position = Position.Forward, ClubId = 6, Nationality = "Portugal" },
                // ── Newcastle United (7) ──
                new Player { Id = 57, FirstName = "Nick", LastName = "Pope", DateOfBirth = new DateTime(1992, 4, 19), Position = Position.Goalkeeper, ClubId = 7, Nationality = "England" },
                new Player { Id = 58, FirstName = "Sven", LastName = "Botman", DateOfBirth = new DateTime(2000, 1, 12), Position = Position.Defender, ClubId = 7, Nationality = "Netherlands" },
                new Player { Id = 59, FirstName = "Kieran", LastName = "Trippier", DateOfBirth = new DateTime(1990, 9, 19), Position = Position.Defender, ClubId = 7, Nationality = "England" },
                new Player { Id = 60, FirstName = "Bruno", LastName = "Guimarães", DateOfBirth = new DateTime(1997, 11, 16), Position = Position.Midfielder, ClubId = 7, Nationality = "Brazil" },
                new Player { Id = 61, FirstName = "Sandro", LastName = "Tonali", DateOfBirth = new DateTime(2000, 5, 8), Position = Position.Midfielder, ClubId = 7, Nationality = "Italy" },
                new Player { Id = 62, FirstName = "Joelinton", LastName = "Cássio", DateOfBirth = new DateTime(1996, 8, 16), Position = Position.Midfielder, ClubId = 7, Nationality = "Brazil" },
                new Player { Id = 63, FirstName = "Anthony", LastName = "Gordon", DateOfBirth = new DateTime(2001, 2, 24), Position = Position.Forward, ClubId = 7, Nationality = "England" },
                new Player { Id = 64, FirstName = "Nick", LastName = "Woltemade", DateOfBirth = new DateTime(2002, 2, 14), Position = Position.Forward, ClubId = 7, Nationality = "Germany" },
                new Player { Id = 65, FirstName = "Anthony", LastName = "Elanga", DateOfBirth = new DateTime(2002, 4, 27), Position = Position.Forward, ClubId = 7, Nationality = "Sweden" },
                // ── Tottenham Hotspur (8) ──
                new Player { Id = 66, FirstName = "Guglielmo", LastName = "Vicario", DateOfBirth = new DateTime(1996, 10, 7), Position = Position.Goalkeeper, ClubId = 8, Nationality = "Italy" },
                new Player { Id = 67, FirstName = "Cristian", LastName = "Romero", DateOfBirth = new DateTime(1998, 4, 27), Position = Position.Defender, ClubId = 8, Nationality = "Argentina" },
                new Player { Id = 68, FirstName = "Micky", LastName = "van de Ven", DateOfBirth = new DateTime(2001, 4, 19), Position = Position.Defender, ClubId = 8, Nationality = "Netherlands" },
                new Player { Id = 69, FirstName = "Pedro", LastName = "Porro", DateOfBirth = new DateTime(1999, 9, 13), Position = Position.Defender, ClubId = 8, Nationality = "Spain" },
                new Player { Id = 70, FirstName = "James", LastName = "Maddison", DateOfBirth = new DateTime(1996, 11, 23), Position = Position.Midfielder, ClubId = 8, Nationality = "England" },
                new Player { Id = 71, FirstName = "Xavi", LastName = "Simons", DateOfBirth = new DateTime(2003, 4, 21), Position = Position.Midfielder, ClubId = 8, Nationality = "Netherlands" },
                new Player { Id = 72, FirstName = "Mohammed", LastName = "Kudus", DateOfBirth = new DateTime(2000, 8, 2), Position = Position.Forward, ClubId = 8, Nationality = "Ghana" },
                new Player { Id = 73, FirstName = "Dominic", LastName = "Solanke", DateOfBirth = new DateTime(1997, 9, 14), Position = Position.Forward, ClubId = 8, Nationality = "England" },
                new Player { Id = 74, FirstName = "Randal", LastName = "Kolo Muani", DateOfBirth = new DateTime(1998, 12, 5), Position = Position.Forward, ClubId = 8, Nationality = "France" },
                // ── FC Barcelona (9) ──
                new Player { Id = 75, FirstName = "Joan", LastName = "García", DateOfBirth = new DateTime(2001, 5, 4), Position = Position.Goalkeeper, ClubId = 9, Nationality = "Spain" },
                new Player { Id = 76, FirstName = "Pau", LastName = "Cubarsí", DateOfBirth = new DateTime(2007, 1, 22), Position = Position.Defender, ClubId = 9, Nationality = "Spain" },
                new Player { Id = 77, FirstName = "Ronald", LastName = "Araújo", DateOfBirth = new DateTime(1999, 3, 7), Position = Position.Defender, ClubId = 9, Nationality = "Uruguay" },
                new Player { Id = 78, FirstName = "Jules", LastName = "Koundé", DateOfBirth = new DateTime(1998, 11, 12), Position = Position.Defender, ClubId = 9, Nationality = "France" },
                new Player { Id = 79, FirstName = "Pedri", LastName = "González", DateOfBirth = new DateTime(2002, 11, 25), Position = Position.Midfielder, ClubId = 9, Nationality = "Spain" },
                new Player { Id = 80, FirstName = "Gavi", LastName = "Páez", DateOfBirth = new DateTime(2004, 8, 5), Position = Position.Midfielder, ClubId = 9, Nationality = "Spain" },
                new Player { Id = 81, FirstName = "Frenkie", LastName = "de Jong", DateOfBirth = new DateTime(1997, 5, 12), Position = Position.Midfielder, ClubId = 9, Nationality = "Netherlands" },
                new Player { Id = 82, FirstName = "Raphinha", LastName = "Dias", DateOfBirth = new DateTime(1996, 12, 14), Position = Position.Forward, ClubId = 9, Nationality = "Brazil" },
                new Player { Id = 83, FirstName = "Lamine", LastName = "Yamal", DateOfBirth = new DateTime(2007, 7, 13), Position = Position.Forward, ClubId = 9, Nationality = "Spain" },
                new Player { Id = 84, FirstName = "Robert", LastName = "Lewandowski", DateOfBirth = new DateTime(1988, 8, 21), Position = Position.Forward, ClubId = 9, Nationality = "Poland" },
                // ── Real Madrid (10) ──
                new Player { Id = 85, FirstName = "Thibaut", LastName = "Courtois", DateOfBirth = new DateTime(1992, 5, 11), Position = Position.Goalkeeper, ClubId = 10, Nationality = "Belgium" },
                new Player { Id = 86, FirstName = "Dani", LastName = "Carvajal", DateOfBirth = new DateTime(1992, 1, 11), Position = Position.Defender, ClubId = 10, Nationality = "Spain" },
                new Player { Id = 87, FirstName = "Éder", LastName = "Militão", DateOfBirth = new DateTime(1998, 1, 18), Position = Position.Defender, ClubId = 10, Nationality = "Brazil" },
                new Player { Id = 88, FirstName = "Antonio", LastName = "Rüdiger", DateOfBirth = new DateTime(1993, 3, 3), Position = Position.Defender, ClubId = 10, Nationality = "Germany" },
                new Player { Id = 89, FirstName = "Trent", LastName = "Alexander-Arnold", DateOfBirth = new DateTime(1998, 10, 7), Position = Position.Defender, ClubId = 10, Nationality = "England" },
                new Player { Id = 90, FirstName = "Aurélien", LastName = "Tchouaméni", DateOfBirth = new DateTime(2000, 1, 27), Position = Position.Midfielder, ClubId = 10, Nationality = "France" },
                new Player { Id = 91, FirstName = "Jude", LastName = "Bellingham", DateOfBirth = new DateTime(2003, 6, 29), Position = Position.Midfielder, ClubId = 10, Nationality = "England" },
                new Player { Id = 92, FirstName = "Federico", LastName = "Valverde", DateOfBirth = new DateTime(1998, 7, 22), Position = Position.Midfielder, ClubId = 10, Nationality = "Uruguay" },
                new Player { Id = 93, FirstName = "Kylian", LastName = "Mbappé", DateOfBirth = new DateTime(1998, 12, 20), Position = Position.Forward, ClubId = 10, Nationality = "France" },
                new Player { Id = 94, FirstName = "Vinícius", LastName = "Júnior", DateOfBirth = new DateTime(2000, 7, 12), Position = Position.Forward, ClubId = 10, Nationality = "Brazil" },
                new Player { Id = 95, FirstName = "Rodrygo", LastName = "Goes", DateOfBirth = new DateTime(2001, 1, 9), Position = Position.Forward, ClubId = 10, Nationality = "Brazil" },
                // ── Atlético Madrid (11) ──
                new Player { Id = 96, FirstName = "Jan", LastName = "Oblak", DateOfBirth = new DateTime(1993, 1, 7), Position = Position.Goalkeeper, ClubId = 11, Nationality = "Slovenia" },
                new Player { Id = 97, FirstName = "Robin", LastName = "Le Normand", DateOfBirth = new DateTime(1996, 11, 11), Position = Position.Defender, ClubId = 11, Nationality = "Spain" },
                new Player { Id = 98, FirstName = "José María", LastName = "Giménez", DateOfBirth = new DateTime(1995, 1, 20), Position = Position.Defender, ClubId = 11, Nationality = "Uruguay" },
                new Player { Id = 99, FirstName = "Nahuel", LastName = "Molina", DateOfBirth = new DateTime(1998, 4, 6), Position = Position.Defender, ClubId = 11, Nationality = "Argentina" },
                new Player { Id = 100, FirstName = "Marcos", LastName = "Llorente", DateOfBirth = new DateTime(1995, 1, 30), Position = Position.Midfielder, ClubId = 11, Nationality = "Spain" },
                new Player { Id = 101, FirstName = "Pablo", LastName = "Barrios", DateOfBirth = new DateTime(2003, 6, 15), Position = Position.Midfielder, ClubId = 11, Nationality = "Spain" },
                new Player { Id = 102, FirstName = "Álex", LastName = "Baena", DateOfBirth = new DateTime(2001, 7, 20), Position = Position.Midfielder, ClubId = 11, Nationality = "Spain" },
                new Player { Id = 103, FirstName = "Antoine", LastName = "Griezmann", DateOfBirth = new DateTime(1991, 3, 21), Position = Position.Forward, ClubId = 11, Nationality = "France" },
                new Player { Id = 104, FirstName = "Julián", LastName = "Álvarez", DateOfBirth = new DateTime(2000, 1, 31), Position = Position.Forward, ClubId = 11, Nationality = "Argentina" },
                new Player { Id = 105, FirstName = "Alexander", LastName = "Sørloth", DateOfBirth = new DateTime(1995, 12, 5), Position = Position.Forward, ClubId = 11, Nationality = "Norway" },
                // ── Villarreal (12) ──
                new Player { Id = 106, FirstName = "Dani", LastName = "Parejo", DateOfBirth = new DateTime(1989, 4, 16), Position = Position.Midfielder, ClubId = 12, Nationality = "Spain" },
                new Player { Id = 107, FirstName = "Juan", LastName = "Foyth", DateOfBirth = new DateTime(1998, 1, 12), Position = Position.Defender, ClubId = 12, Nationality = "Argentina" },
                new Player { Id = 108, FirstName = "Thomas", LastName = "Partey", DateOfBirth = new DateTime(1993, 6, 13), Position = Position.Midfielder, ClubId = 12, Nationality = "Ghana" },
                new Player { Id = 109, FirstName = "Gerard", LastName = "Moreno", DateOfBirth = new DateTime(1992, 4, 7), Position = Position.Forward, ClubId = 12, Nationality = "Spain" },
                new Player { Id = 110, FirstName = "Ayoze", LastName = "Pérez", DateOfBirth = new DateTime(1993, 7, 29), Position = Position.Forward, ClubId = 12, Nationality = "Spain" },
                new Player { Id = 111, FirstName = "Nicolas", LastName = "Pépé", DateOfBirth = new DateTime(1995, 5, 29), Position = Position.Forward, ClubId = 12, Nationality = "Ivory Coast" },
                new Player { Id = 112, FirstName = "Alfonso", LastName = "Pedraza", DateOfBirth = new DateTime(1996, 4, 9), Position = Position.Defender, ClubId = 12, Nationality = "Spain" },
                // ── Real Betis (13) ──
                new Player { Id = 113, FirstName = "Pau", LastName = "López", DateOfBirth = new DateTime(1995, 12, 13), Position = Position.Goalkeeper, ClubId = 13, Nationality = "Spain" },
                new Player { Id = 114, FirstName = "Héctor", LastName = "Bellerín", DateOfBirth = new DateTime(1995, 3, 19), Position = Position.Defender, ClubId = 13, Nationality = "Spain" },
                new Player { Id = 115, FirstName = "Marc", LastName = "Bartra", DateOfBirth = new DateTime(1991, 1, 15), Position = Position.Defender, ClubId = 13, Nationality = "Spain" },
                new Player { Id = 116, FirstName = "Isco", LastName = "Alarcón", DateOfBirth = new DateTime(1992, 4, 21), Position = Position.Midfielder, ClubId = 13, Nationality = "Spain" },
                new Player { Id = 117, FirstName = "Pablo", LastName = "Fornals", DateOfBirth = new DateTime(1996, 2, 10), Position = Position.Midfielder, ClubId = 13, Nationality = "Spain" },
                new Player { Id = 118, FirstName = "Giovani", LastName = "Lo Celso", DateOfBirth = new DateTime(1996, 4, 9), Position = Position.Midfielder, ClubId = 13, Nationality = "Argentina" },
                new Player { Id = 119, FirstName = "Antony", LastName = "Santos", DateOfBirth = new DateTime(2000, 2, 24), Position = Position.Forward, ClubId = 13, Nationality = "Brazil" },
                new Player { Id = 120, FirstName = "Cucho", LastName = "Hernández", DateOfBirth = new DateTime(1999, 4, 23), Position = Position.Forward, ClubId = 13, Nationality = "Colombia" },
                new Player { Id = 121, FirstName = "Abde", LastName = "Ezzalzouli", DateOfBirth = new DateTime(2001, 12, 17), Position = Position.Forward, ClubId = 13, Nationality = "Morocco" },
                // ── Athletic Bilbao (14) ──
                new Player { Id = 122, FirstName = "Unai", LastName = "Simón", DateOfBirth = new DateTime(1997, 6, 11), Position = Position.Goalkeeper, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 123, FirstName = "Dani", LastName = "Vivian", DateOfBirth = new DateTime(1999, 7, 5), Position = Position.Defender, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 124, FirstName = "Yeray", LastName = "Álvarez", DateOfBirth = new DateTime(1995, 1, 24), Position = Position.Defender, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 125, FirstName = "Aymeric", LastName = "Laporte", DateOfBirth = new DateTime(1994, 5, 27), Position = Position.Defender, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 126, FirstName = "Oihan", LastName = "Sancet", DateOfBirth = new DateTime(2000, 4, 25), Position = Position.Midfielder, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 127, FirstName = "Iñaki", LastName = "Williams", DateOfBirth = new DateTime(1994, 6, 15), Position = Position.Forward, ClubId = 14, Nationality = "Ghana" },
                new Player { Id = 128, FirstName = "Nico", LastName = "Williams", DateOfBirth = new DateTime(2002, 7, 12), Position = Position.Forward, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 129, FirstName = "Álex", LastName = "Berenguer", DateOfBirth = new DateTime(1995, 7, 4), Position = Position.Forward, ClubId = 14, Nationality = "Spain" },
                new Player { Id = 130, FirstName = "Gorka", LastName = "Guruzeta", DateOfBirth = new DateTime(1996, 9, 12), Position = Position.Forward, ClubId = 14, Nationality = "Spain" },
                // ── Real Sociedad (15) ──
                new Player { Id = 131, FirstName = "Álex", LastName = "Remiro", DateOfBirth = new DateTime(1995, 3, 24), Position = Position.Goalkeeper, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 132, FirstName = "Igor", LastName = "Zubeldia", DateOfBirth = new DateTime(1997, 3, 30), Position = Position.Defender, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 133, FirstName = "Aritz", LastName = "Elustondo", DateOfBirth = new DateTime(1994, 3, 28), Position = Position.Defender, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 134, FirstName = "Brais", LastName = "Méndez", DateOfBirth = new DateTime(1997, 1, 7), Position = Position.Midfielder, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 135, FirstName = "Takefusa", LastName = "Kubo", DateOfBirth = new DateTime(2001, 6, 4), Position = Position.Forward, ClubId = 15, Nationality = "Japan" },
                new Player { Id = 136, FirstName = "Mikel", LastName = "Oyarzabal", DateOfBirth = new DateTime(1997, 4, 21), Position = Position.Forward, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 137, FirstName = "Ander", LastName = "Barrenetxea", DateOfBirth = new DateTime(2001, 12, 27), Position = Position.Forward, ClubId = 15, Nationality = "Spain" },
                new Player { Id = 138, FirstName = "Luka", LastName = "Sučić", DateOfBirth = new DateTime(2002, 9, 8), Position = Position.Midfielder, ClubId = 15, Nationality = "Croatia" },
                // ── Sevilla (16) ──
                new Player { Id = 139, FirstName = "Nemanja", LastName = "Gudelj", DateOfBirth = new DateTime(1991, 11, 16), Position = Position.Midfielder, ClubId = 16, Nationality = "Serbia" },
                new Player { Id = 140, FirstName = "César", LastName = "Azpilicueta", DateOfBirth = new DateTime(1989, 8, 28), Position = Position.Defender, ClubId = 16, Nationality = "Spain" },
                new Player { Id = 141, FirstName = "Tanguy", LastName = "Nianzou", DateOfBirth = new DateTime(2002, 6, 7), Position = Position.Defender, ClubId = 16, Nationality = "France" },
                new Player { Id = 142, FirstName = "Gabriel", LastName = "Suazo", DateOfBirth = new DateTime(1997, 8, 9), Position = Position.Defender, ClubId = 16, Nationality = "Chile" },
                new Player { Id = 143, FirstName = "Djibril", LastName = "Sow", DateOfBirth = new DateTime(1997, 2, 6), Position = Position.Midfielder, ClubId = 16, Nationality = "Switzerland" },
                new Player { Id = 144, FirstName = "Rubén", LastName = "Vargas", DateOfBirth = new DateTime(1998, 8, 5), Position = Position.Forward, ClubId = 16, Nationality = "Switzerland" },
                new Player { Id = 145, FirstName = "Isaac", LastName = "Romero", DateOfBirth = new DateTime(2000, 5, 18), Position = Position.Forward, ClubId = 16, Nationality = "Spain" },
                new Player { Id = 146, FirstName = "Alexis", LastName = "Sánchez", DateOfBirth = new DateTime(1988, 12, 19), Position = Position.Forward, ClubId = 16, Nationality = "Chile" },
                // ── Inter Milan (17) ──
                new Player { Id = 147, FirstName = "Yann", LastName = "Sommer", DateOfBirth = new DateTime(1988, 12, 17), Position = Position.Goalkeeper, ClubId = 17, Nationality = "Switzerland" },
                new Player { Id = 148, FirstName = "Alessandro", LastName = "Bastoni", DateOfBirth = new DateTime(1999, 4, 13), Position = Position.Defender, ClubId = 17, Nationality = "Italy" },
                new Player { Id = 149, FirstName = "Federico", LastName = "Dimarco", DateOfBirth = new DateTime(1997, 11, 10), Position = Position.Defender, ClubId = 17, Nationality = "Italy" },
                new Player { Id = 150, FirstName = "Denzel", LastName = "Dumfries", DateOfBirth = new DateTime(1996, 4, 18), Position = Position.Defender, ClubId = 17, Nationality = "Netherlands" },
                new Player { Id = 151, FirstName = "Nicolò", LastName = "Barella", DateOfBirth = new DateTime(1997, 2, 7), Position = Position.Midfielder, ClubId = 17, Nationality = "Italy" },
                new Player { Id = 152, FirstName = "Hakan", LastName = "Çalhanoğlu", DateOfBirth = new DateTime(1994, 2, 8), Position = Position.Midfielder, ClubId = 17, Nationality = "Turkey" },
                new Player { Id = 153, FirstName = "Lautaro", LastName = "Martínez", DateOfBirth = new DateTime(1997, 8, 22), Position = Position.Forward, ClubId = 17, Nationality = "Argentina" },
                new Player { Id = 154, FirstName = "Marcus", LastName = "Thuram", DateOfBirth = new DateTime(1997, 8, 6), Position = Position.Forward, ClubId = 17, Nationality = "France" },
                new Player { Id = 155, FirstName = "Francesco Pio", LastName = "Esposito", DateOfBirth = new DateTime(2005, 6, 28), Position = Position.Forward, ClubId = 17, Nationality = "Italy" },
                // ── AC Milan (18) ──
                new Player { Id = 156, FirstName = "Mike", LastName = "Maignan", DateOfBirth = new DateTime(1995, 7, 3), Position = Position.Goalkeeper, ClubId = 18, Nationality = "France" },
                new Player { Id = 157, FirstName = "Fikayo", LastName = "Tomori", DateOfBirth = new DateTime(1997, 12, 19), Position = Position.Defender, ClubId = 18, Nationality = "England" },
                new Player { Id = 158, FirstName = "Strahinja", LastName = "Pavlović", DateOfBirth = new DateTime(2001, 5, 24), Position = Position.Defender, ClubId = 18, Nationality = "Serbia" },
                new Player { Id = 159, FirstName = "Youssouf", LastName = "Fofana", DateOfBirth = new DateTime(1999, 1, 10), Position = Position.Midfielder, ClubId = 18, Nationality = "France" },
                new Player { Id = 160, FirstName = "Luka", LastName = "Modrić", DateOfBirth = new DateTime(1985, 9, 9), Position = Position.Midfielder, ClubId = 18, Nationality = "Croatia" },
                new Player { Id = 161, FirstName = "Adrien", LastName = "Rabiot", DateOfBirth = new DateTime(1995, 4, 3), Position = Position.Midfielder, ClubId = 18, Nationality = "France" },
                new Player { Id = 162, FirstName = "Christian", LastName = "Pulisic", DateOfBirth = new DateTime(1998, 9, 18), Position = Position.Forward, ClubId = 18, Nationality = "United States" },
                new Player { Id = 163, FirstName = "Rafael", LastName = "Leão", DateOfBirth = new DateTime(1999, 6, 10), Position = Position.Forward, ClubId = 18, Nationality = "Portugal" },
                new Player { Id = 164, FirstName = "Santiago", LastName = "Giménez", DateOfBirth = new DateTime(2001, 4, 18), Position = Position.Forward, ClubId = 18, Nationality = "Mexico" },
                // ── Juventus (19) ──
                new Player { Id = 165, FirstName = "Michele", LastName = "Di Gregorio", DateOfBirth = new DateTime(1997, 7, 27), Position = Position.Goalkeeper, ClubId = 19, Nationality = "Italy" },
                new Player { Id = 166, FirstName = "Gleison", LastName = "Bremer", DateOfBirth = new DateTime(1997, 3, 18), Position = Position.Defender, ClubId = 19, Nationality = "Brazil" },
                new Player { Id = 167, FirstName = "Federico", LastName = "Gatti", DateOfBirth = new DateTime(1998, 6, 24), Position = Position.Defender, ClubId = 19, Nationality = "Italy" },
                new Player { Id = 168, FirstName = "Andrea", LastName = "Cambiaso", DateOfBirth = new DateTime(2000, 2, 20), Position = Position.Defender, ClubId = 19, Nationality = "Italy" },
                new Player { Id = 169, FirstName = "Manuel", LastName = "Locatelli", DateOfBirth = new DateTime(1998, 1, 8), Position = Position.Midfielder, ClubId = 19, Nationality = "Italy" },
                new Player { Id = 170, FirstName = "Khéphren", LastName = "Thuram", DateOfBirth = new DateTime(2001, 3, 26), Position = Position.Midfielder, ClubId = 19, Nationality = "France" },
                new Player { Id = 171, FirstName = "Kenan", LastName = "Yıldız", DateOfBirth = new DateTime(2005, 5, 4), Position = Position.Forward, ClubId = 19, Nationality = "Turkey" },
                new Player { Id = 172, FirstName = "Dušan", LastName = "Vlahović", DateOfBirth = new DateTime(2000, 1, 28), Position = Position.Forward, ClubId = 19, Nationality = "Serbia" },
                new Player { Id = 173, FirstName = "Jonathan", LastName = "David", DateOfBirth = new DateTime(2000, 1, 14), Position = Position.Forward, ClubId = 19, Nationality = "Canada" },
                // ── Napoli (20) ──
                new Player { Id = 174, FirstName = "Alex", LastName = "Meret", DateOfBirth = new DateTime(1997, 3, 22), Position = Position.Goalkeeper, ClubId = 20, Nationality = "Italy" },
                new Player { Id = 175, FirstName = "Giovanni", LastName = "Di Lorenzo", DateOfBirth = new DateTime(1993, 8, 4), Position = Position.Defender, ClubId = 20, Nationality = "Italy" },
                new Player { Id = 176, FirstName = "Amir", LastName = "Rrahmani", DateOfBirth = new DateTime(1994, 2, 24), Position = Position.Defender, ClubId = 20, Nationality = "Kosovo" },
                new Player { Id = 177, FirstName = "Alessandro", LastName = "Buongiorno", DateOfBirth = new DateTime(1999, 6, 6), Position = Position.Defender, ClubId = 20, Nationality = "Italy" },
                new Player { Id = 178, FirstName = "Stanislav", LastName = "Lobotka", DateOfBirth = new DateTime(1994, 11, 25), Position = Position.Midfielder, ClubId = 20, Nationality = "Slovakia" },
                new Player { Id = 179, FirstName = "Scott", LastName = "McTominay", DateOfBirth = new DateTime(1996, 12, 8), Position = Position.Midfielder, ClubId = 20, Nationality = "Scotland" },
                new Player { Id = 180, FirstName = "Kevin", LastName = "De Bruyne", DateOfBirth = new DateTime(1991, 6, 28), Position = Position.Midfielder, ClubId = 20, Nationality = "Belgium" },
                new Player { Id = 181, FirstName = "Romelu", LastName = "Lukaku", DateOfBirth = new DateTime(1993, 5, 13), Position = Position.Forward, ClubId = 20, Nationality = "Belgium" },
                new Player { Id = 182, FirstName = "Matteo", LastName = "Politano", DateOfBirth = new DateTime(1993, 8, 3), Position = Position.Forward, ClubId = 20, Nationality = "Italy" },
                // ── Roma (21) ──
                new Player { Id = 183, FirstName = "Mile", LastName = "Svilar", DateOfBirth = new DateTime(1999, 8, 27), Position = Position.Goalkeeper, ClubId = 21, Nationality = "Serbia" },
                new Player { Id = 184, FirstName = "Gianluca", LastName = "Mancini", DateOfBirth = new DateTime(1996, 4, 17), Position = Position.Defender, ClubId = 21, Nationality = "Italy" },
                new Player { Id = 185, FirstName = "Evan", LastName = "Ndicka", DateOfBirth = new DateTime(1999, 8, 20), Position = Position.Defender, ClubId = 21, Nationality = "Ivory Coast" },
                new Player { Id = 186, FirstName = "Bryan", LastName = "Cristante", DateOfBirth = new DateTime(1995, 3, 3), Position = Position.Midfielder, ClubId = 21, Nationality = "Italy" },
                new Player { Id = 187, FirstName = "Manu", LastName = "Koné", DateOfBirth = new DateTime(2001, 5, 17), Position = Position.Midfielder, ClubId = 21, Nationality = "France" },
                new Player { Id = 188, FirstName = "Lorenzo", LastName = "Pellegrini", DateOfBirth = new DateTime(1996, 6, 19), Position = Position.Midfielder, ClubId = 21, Nationality = "Italy" },
                new Player { Id = 189, FirstName = "Paulo", LastName = "Dybala", DateOfBirth = new DateTime(1993, 11, 15), Position = Position.Forward, ClubId = 21, Nationality = "Argentina" },
                new Player { Id = 190, FirstName = "Matías", LastName = "Soulé", DateOfBirth = new DateTime(2003, 4, 15), Position = Position.Forward, ClubId = 21, Nationality = "Argentina" },
                new Player { Id = 191, FirstName = "Artem", LastName = "Dovbyk", DateOfBirth = new DateTime(1997, 6, 21), Position = Position.Forward, ClubId = 21, Nationality = "Ukraine" },
                // ── Atalanta (22) ──
                new Player { Id = 192, FirstName = "Marco", LastName = "Carnesecchi", DateOfBirth = new DateTime(2000, 7, 1), Position = Position.Goalkeeper, ClubId = 22, Nationality = "Italy" },
                new Player { Id = 193, FirstName = "Isak", LastName = "Hien", DateOfBirth = new DateTime(1999, 1, 13), Position = Position.Defender, ClubId = 22, Nationality = "Sweden" },
                new Player { Id = 194, FirstName = "Berat", LastName = "Djimsiti", DateOfBirth = new DateTime(1993, 2, 19), Position = Position.Defender, ClubId = 22, Nationality = "Albania" },
                new Player { Id = 195, FirstName = "Raoul", LastName = "Bellanova", DateOfBirth = new DateTime(2000, 5, 17), Position = Position.Defender, ClubId = 22, Nationality = "Italy" },
                new Player { Id = 196, FirstName = "Marten", LastName = "de Roon", DateOfBirth = new DateTime(1991, 3, 29), Position = Position.Midfielder, ClubId = 22, Nationality = "Netherlands" },
                new Player { Id = 197, FirstName = "Éderson", LastName = "dos Santos", DateOfBirth = new DateTime(1999, 7, 7), Position = Position.Midfielder, ClubId = 22, Nationality = "Brazil" },
                new Player { Id = 198, FirstName = "Charles", LastName = "De Ketelaere", DateOfBirth = new DateTime(2001, 3, 10), Position = Position.Midfielder, ClubId = 22, Nationality = "Belgium" },
                new Player { Id = 199, FirstName = "Gianluca", LastName = "Scamacca", DateOfBirth = new DateTime(1999, 1, 1), Position = Position.Forward, ClubId = 22, Nationality = "Italy" },
                new Player { Id = 200, FirstName = "Nikola", LastName = "Krstović", DateOfBirth = new DateTime(2000, 4, 5), Position = Position.Forward, ClubId = 22, Nationality = "Montenegro" },
                // ── Lazio (23) ──
                new Player { Id = 201, FirstName = "Ivan", LastName = "Provedel", DateOfBirth = new DateTime(1994, 3, 17), Position = Position.Goalkeeper, ClubId = 23, Nationality = "Italy" },
                new Player { Id = 202, FirstName = "Alessio", LastName = "Romagnoli", DateOfBirth = new DateTime(1995, 1, 12), Position = Position.Defender, ClubId = 23, Nationality = "Italy" },
                new Player { Id = 203, FirstName = "Mario", LastName = "Gila", DateOfBirth = new DateTime(2000, 8, 29), Position = Position.Defender, ClubId = 23, Nationality = "Spain" },
                new Player { Id = 204, FirstName = "Nuno", LastName = "Tavares", DateOfBirth = new DateTime(2000, 1, 26), Position = Position.Defender, ClubId = 23, Nationality = "Portugal" },
                new Player { Id = 205, FirstName = "Nicolò", LastName = "Rovella", DateOfBirth = new DateTime(2001, 12, 4), Position = Position.Midfielder, ClubId = 23, Nationality = "Italy" },
                new Player { Id = 206, FirstName = "Mattia", LastName = "Zaccagni", DateOfBirth = new DateTime(1995, 6, 16), Position = Position.Forward, ClubId = 23, Nationality = "Italy" },
                new Player { Id = 207, FirstName = "Pedro", LastName = "Rodríguez", DateOfBirth = new DateTime(1987, 7, 28), Position = Position.Forward, ClubId = 23, Nationality = "Spain" },
                new Player { Id = 208, FirstName = "Valentín", LastName = "Castellanos", DateOfBirth = new DateTime(1998, 10, 3), Position = Position.Forward, ClubId = 23, Nationality = "Argentina" },
                new Player { Id = 209, FirstName = "Gustav", LastName = "Isaksen", DateOfBirth = new DateTime(2001, 4, 19), Position = Position.Forward, ClubId = 23, Nationality = "Denmark" },
                // ── Fiorentina (24) ──
                new Player { Id = 210, FirstName = "David", LastName = "de Gea", DateOfBirth = new DateTime(1990, 11, 7), Position = Position.Goalkeeper, ClubId = 24, Nationality = "Spain" },
                new Player { Id = 211, FirstName = "Dodô", LastName = "Silva", DateOfBirth = new DateTime(1998, 11, 17), Position = Position.Defender, ClubId = 24, Nationality = "Brazil" },
                new Player { Id = 212, FirstName = "Daniele", LastName = "Rugani", DateOfBirth = new DateTime(1994, 7, 29), Position = Position.Defender, ClubId = 24, Nationality = "Italy" },
                new Player { Id = 213, FirstName = "Robin", LastName = "Gosens", DateOfBirth = new DateTime(1994, 7, 5), Position = Position.Defender, ClubId = 24, Nationality = "Germany" },
                new Player { Id = 214, FirstName = "Rolando", LastName = "Mandragora", DateOfBirth = new DateTime(1997, 6, 29), Position = Position.Midfielder, ClubId = 24, Nationality = "Italy" },
                new Player { Id = 215, FirstName = "Nicolò", LastName = "Fagioli", DateOfBirth = new DateTime(2001, 2, 12), Position = Position.Midfielder, ClubId = 24, Nationality = "Italy" },
                new Player { Id = 216, FirstName = "Moise", LastName = "Kean", DateOfBirth = new DateTime(2000, 2, 28), Position = Position.Forward, ClubId = 24, Nationality = "Italy" },
                new Player { Id = 217, FirstName = "Albert", LastName = "Guðmundsson", DateOfBirth = new DateTime(1997, 6, 6), Position = Position.Forward, ClubId = 24, Nationality = "Iceland" },
                new Player { Id = 218, FirstName = "Roberto", LastName = "Piccoli", DateOfBirth = new DateTime(2001, 1, 27), Position = Position.Forward, ClubId = 24, Nationality = "Italy" },
                // ── Bayern Munich (25) ──
                new Player { Id = 219, FirstName = "Manuel", LastName = "Neuer", DateOfBirth = new DateTime(1986, 3, 27), Position = Position.Goalkeeper, ClubId = 25, Nationality = "Germany" },
                new Player { Id = 220, FirstName = "Dayot", LastName = "Upamecano", DateOfBirth = new DateTime(1998, 10, 27), Position = Position.Defender, ClubId = 25, Nationality = "France" },
                new Player { Id = 221, FirstName = "Jonathan", LastName = "Tah", DateOfBirth = new DateTime(1996, 2, 11), Position = Position.Defender, ClubId = 25, Nationality = "Germany" },
                new Player { Id = 222, FirstName = "Alphonso", LastName = "Davies", DateOfBirth = new DateTime(2000, 11, 2), Position = Position.Defender, ClubId = 25, Nationality = "Canada" },
                new Player { Id = 223, FirstName = "Joshua", LastName = "Kimmich", DateOfBirth = new DateTime(1995, 2, 8), Position = Position.Midfielder, ClubId = 25, Nationality = "Germany" },
                new Player { Id = 224, FirstName = "Leon", LastName = "Goretzka", DateOfBirth = new DateTime(1995, 2, 6), Position = Position.Midfielder, ClubId = 25, Nationality = "Germany" },
                new Player { Id = 225, FirstName = "Jamal", LastName = "Musiala", DateOfBirth = new DateTime(2003, 2, 26), Position = Position.Midfielder, ClubId = 25, Nationality = "Germany" },
                new Player { Id = 226, FirstName = "Michael", LastName = "Olise", DateOfBirth = new DateTime(2001, 12, 12), Position = Position.Forward, ClubId = 25, Nationality = "France" },
                new Player { Id = 227, FirstName = "Harry", LastName = "Kane", DateOfBirth = new DateTime(1993, 7, 28), Position = Position.Forward, ClubId = 25, Nationality = "England" },
                new Player { Id = 228, FirstName = "Luis", LastName = "Díaz", DateOfBirth = new DateTime(1997, 1, 13), Position = Position.Forward, ClubId = 25, Nationality = "Colombia" },
                // ── Borussia Dortmund (26) ──
                new Player { Id = 229, FirstName = "Gregor", LastName = "Kobel", DateOfBirth = new DateTime(1997, 12, 6), Position = Position.Goalkeeper, ClubId = 26, Nationality = "Switzerland" },
                new Player { Id = 230, FirstName = "Nico", LastName = "Schlotterbeck", DateOfBirth = new DateTime(1999, 12, 1), Position = Position.Defender, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 231, FirstName = "Niklas", LastName = "Süle", DateOfBirth = new DateTime(1995, 9, 3), Position = Position.Defender, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 232, FirstName = "Waldemar", LastName = "Anton", DateOfBirth = new DateTime(1996, 7, 20), Position = Position.Defender, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 233, FirstName = "Emre", LastName = "Can", DateOfBirth = new DateTime(1994, 1, 12), Position = Position.Midfielder, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 234, FirstName = "Julian", LastName = "Brandt", DateOfBirth = new DateTime(1996, 5, 2), Position = Position.Midfielder, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 235, FirstName = "Jobe", LastName = "Bellingham", DateOfBirth = new DateTime(2005, 9, 23), Position = Position.Midfielder, ClubId = 26, Nationality = "England" },
                new Player { Id = 236, FirstName = "Karim", LastName = "Adeyemi", DateOfBirth = new DateTime(2002, 1, 18), Position = Position.Forward, ClubId = 26, Nationality = "Germany" },
                new Player { Id = 237, FirstName = "Serhou", LastName = "Guirassy", DateOfBirth = new DateTime(1996, 3, 12), Position = Position.Forward, ClubId = 26, Nationality = "Guinea" },
                new Player { Id = 238, FirstName = "Maximilian", LastName = "Beier", DateOfBirth = new DateTime(2002, 10, 17), Position = Position.Forward, ClubId = 26, Nationality = "Germany" },
                // ── RB Leipzig (27) ──
                new Player { Id = 239, FirstName = "Péter", LastName = "Gulácsi", DateOfBirth = new DateTime(1990, 5, 6), Position = Position.Goalkeeper, ClubId = 27, Nationality = "Hungary" },
                new Player { Id = 240, FirstName = "Willi", LastName = "Orbán", DateOfBirth = new DateTime(1992, 11, 3), Position = Position.Defender, ClubId = 27, Nationality = "Hungary" },
                new Player { Id = 241, FirstName = "Castello", LastName = "Lukeba", DateOfBirth = new DateTime(2002, 12, 17), Position = Position.Defender, ClubId = 27, Nationality = "France" },
                new Player { Id = 242, FirstName = "David", LastName = "Raum", DateOfBirth = new DateTime(1998, 4, 22), Position = Position.Defender, ClubId = 27, Nationality = "Germany" },
                new Player { Id = 243, FirstName = "Xaver", LastName = "Schlager", DateOfBirth = new DateTime(1997, 9, 28), Position = Position.Midfielder, ClubId = 27, Nationality = "Austria" },
                new Player { Id = 244, FirstName = "Christoph", LastName = "Baumgartner", DateOfBirth = new DateTime(1999, 8, 1), Position = Position.Midfielder, ClubId = 27, Nationality = "Austria" },
                new Player { Id = 245, FirstName = "Nicolas", LastName = "Seiwald", DateOfBirth = new DateTime(2001, 5, 4), Position = Position.Midfielder, ClubId = 27, Nationality = "Austria" },
                new Player { Id = 246, FirstName = "Antonio", LastName = "Nusa", DateOfBirth = new DateTime(2005, 4, 17), Position = Position.Forward, ClubId = 27, Nationality = "Norway" },
                new Player { Id = 247, FirstName = "Johan", LastName = "Bakayoko", DateOfBirth = new DateTime(2003, 4, 20), Position = Position.Forward, ClubId = 27, Nationality = "Belgium" },
                new Player { Id = 248, FirstName = "Conrad", LastName = "Harder", DateOfBirth = new DateTime(2005, 4, 18), Position = Position.Forward, ClubId = 27, Nationality = "Denmark" },
                // ── Bayer Leverkusen (28) ──
                new Player { Id = 249, FirstName = "Mark", LastName = "Flekken", DateOfBirth = new DateTime(1993, 6, 13), Position = Position.Goalkeeper, ClubId = 28, Nationality = "Netherlands" },
                new Player { Id = 250, FirstName = "Edmond", LastName = "Tapsoba", DateOfBirth = new DateTime(1999, 2, 2), Position = Position.Defender, ClubId = 28, Nationality = "Burkina Faso" },
                new Player { Id = 251, FirstName = "Loïc", LastName = "Badé", DateOfBirth = new DateTime(2000, 4, 11), Position = Position.Defender, ClubId = 28, Nationality = "France" },
                new Player { Id = 252, FirstName = "Álex", LastName = "Grimaldo", DateOfBirth = new DateTime(1995, 9, 20), Position = Position.Defender, ClubId = 28, Nationality = "Spain" },
                new Player { Id = 253, FirstName = "Robert", LastName = "Andrich", DateOfBirth = new DateTime(1994, 9, 22), Position = Position.Midfielder, ClubId = 28, Nationality = "Germany" },
                new Player { Id = 254, FirstName = "Aleix", LastName = "García", DateOfBirth = new DateTime(1997, 6, 28), Position = Position.Midfielder, ClubId = 28, Nationality = "Spain" },
                new Player { Id = 255, FirstName = "Exequiel", LastName = "Palacios", DateOfBirth = new DateTime(1998, 10, 5), Position = Position.Midfielder, ClubId = 28, Nationality = "Argentina" },
                new Player { Id = 256, FirstName = "Malik", LastName = "Tillman", DateOfBirth = new DateTime(2002, 5, 28), Position = Position.Midfielder, ClubId = 28, Nationality = "United States" },
                new Player { Id = 257, FirstName = "Patrik", LastName = "Schick", DateOfBirth = new DateTime(1996, 1, 24), Position = Position.Forward, ClubId = 28, Nationality = "Czech Republic" },
                new Player { Id = 258, FirstName = "Martin", LastName = "Terrier", DateOfBirth = new DateTime(1997, 3, 4), Position = Position.Forward, ClubId = 28, Nationality = "France" },
                // ── VfB Stuttgart (29) ──
                new Player { Id = 259, FirstName = "Alexander", LastName = "Nübel", DateOfBirth = new DateTime(1996, 9, 30), Position = Position.Goalkeeper, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 260, FirstName = "Jeff", LastName = "Chabot", DateOfBirth = new DateTime(1998, 2, 12), Position = Position.Defender, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 261, FirstName = "Maximilian", LastName = "Mittelstädt", DateOfBirth = new DateTime(1997, 3, 18), Position = Position.Defender, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 262, FirstName = "Angelo", LastName = "Stiller", DateOfBirth = new DateTime(2001, 4, 4), Position = Position.Midfielder, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 263, FirstName = "Chris", LastName = "Führich", DateOfBirth = new DateTime(1998, 1, 9), Position = Position.Midfielder, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 264, FirstName = "Atakan", LastName = "Karazor", DateOfBirth = new DateTime(1996, 10, 13), Position = Position.Midfielder, ClubId = 29, Nationality = "Turkey" },
                new Player { Id = 265, FirstName = "Bilal", LastName = "El Khannouss", DateOfBirth = new DateTime(2004, 5, 10), Position = Position.Midfielder, ClubId = 29, Nationality = "Morocco" },
                new Player { Id = 266, FirstName = "Deniz", LastName = "Undav", DateOfBirth = new DateTime(1996, 7, 19), Position = Position.Forward, ClubId = 29, Nationality = "Germany" },
                new Player { Id = 267, FirstName = "Ermedin", LastName = "Demirović", DateOfBirth = new DateTime(1998, 3, 25), Position = Position.Forward, ClubId = 29, Nationality = "Bosnia and Herzegovina" },
                new Player { Id = 268, FirstName = "Jamie", LastName = "Leweling", DateOfBirth = new DateTime(2001, 2, 26), Position = Position.Forward, ClubId = 29, Nationality = "Germany" },
                // ── Eintracht Frankfurt (30) ──
                new Player { Id = 269, FirstName = "Michael", LastName = "Zetterer", DateOfBirth = new DateTime(1995, 7, 12), Position = Position.Goalkeeper, ClubId = 30, Nationality = "Germany" },
                new Player { Id = 270, FirstName = "Robin", LastName = "Koch", DateOfBirth = new DateTime(1996, 7, 17), Position = Position.Defender, ClubId = 30, Nationality = "Germany" },
                new Player { Id = 271, FirstName = "Arthur", LastName = "Theate", DateOfBirth = new DateTime(2000, 5, 25), Position = Position.Defender, ClubId = 30, Nationality = "Belgium" },
                new Player { Id = 272, FirstName = "Rasmus", LastName = "Kristensen", DateOfBirth = new DateTime(1997, 7, 11), Position = Position.Defender, ClubId = 30, Nationality = "Denmark" },
                new Player { Id = 273, FirstName = "Ellyes", LastName = "Skhiri", DateOfBirth = new DateTime(1995, 5, 10), Position = Position.Midfielder, ClubId = 30, Nationality = "Tunisia" },
                new Player { Id = 274, FirstName = "Hugo", LastName = "Larsson", DateOfBirth = new DateTime(2004, 6, 27), Position = Position.Midfielder, ClubId = 30, Nationality = "Sweden" },
                new Player { Id = 275, FirstName = "Mario", LastName = "Götze", DateOfBirth = new DateTime(1992, 6, 3), Position = Position.Midfielder, ClubId = 30, Nationality = "Germany" },
                new Player { Id = 276, FirstName = "Ritsu", LastName = "Dōan", DateOfBirth = new DateTime(1998, 6, 16), Position = Position.Forward, ClubId = 30, Nationality = "Japan" },
                new Player { Id = 277, FirstName = "Jonathan", LastName = "Burkardt", DateOfBirth = new DateTime(2000, 7, 11), Position = Position.Forward, ClubId = 30, Nationality = "Germany" },
                new Player { Id = 278, FirstName = "Ansgar", LastName = "Knauff", DateOfBirth = new DateTime(2002, 1, 10), Position = Position.Forward, ClubId = 30, Nationality = "Germany" },
                // ── Borussia Mönchengladbach (31) ──
                new Player { Id = 279, FirstName = "Tim", LastName = "Kleindienst", DateOfBirth = new DateTime(1995, 8, 31), Position = Position.Forward, ClubId = 31, Nationality = "Germany" },
                new Player { Id = 280, FirstName = "Franck", LastName = "Honorat", DateOfBirth = new DateTime(1996, 8, 11), Position = Position.Forward, ClubId = 31, Nationality = "France" },
                new Player { Id = 281, FirstName = "Florian", LastName = "Neuhaus", DateOfBirth = new DateTime(1997, 3, 16), Position = Position.Midfielder, ClubId = 31, Nationality = "Germany" },
                new Player { Id = 282, FirstName = "Rocco", LastName = "Reitz", DateOfBirth = new DateTime(2002, 5, 29), Position = Position.Midfielder, ClubId = 31, Nationality = "Germany" },
                new Player { Id = 283, FirstName = "Nico", LastName = "Elvedi", DateOfBirth = new DateTime(1996, 9, 30), Position = Position.Defender, ClubId = 31, Nationality = "Switzerland" },
                new Player { Id = 284, FirstName = "Kevin", LastName = "Stöger", DateOfBirth = new DateTime(1993, 8, 27), Position = Position.Midfielder, ClubId = 31, Nationality = "Austria" },
                new Player { Id = 285, FirstName = "Giovanni", LastName = "Reyna", DateOfBirth = new DateTime(2002, 11, 13), Position = Position.Midfielder, ClubId = 31, Nationality = "United States" },
                new Player { Id = 286, FirstName = "Joe", LastName = "Scally", DateOfBirth = new DateTime(2002, 12, 31), Position = Position.Defender, ClubId = 31, Nationality = "United States" },
                // ── Paris Saint-Germain (32) ──
                new Player { Id = 287, FirstName = "Lucas", LastName = "Chevalier", DateOfBirth = new DateTime(2001, 11, 6), Position = Position.Goalkeeper, ClubId = 32, Nationality = "France" },
                new Player { Id = 288, FirstName = "Achraf", LastName = "Hakimi", DateOfBirth = new DateTime(1998, 11, 4), Position = Position.Defender, ClubId = 32, Nationality = "Morocco" },
                new Player { Id = 289, FirstName = "Marquinhos", LastName = "Corrêa", DateOfBirth = new DateTime(1994, 5, 14), Position = Position.Defender, ClubId = 32, Nationality = "Brazil" },
                new Player { Id = 290, FirstName = "Nuno", LastName = "Mendes", DateOfBirth = new DateTime(2002, 6, 19), Position = Position.Defender, ClubId = 32, Nationality = "Portugal" },
                new Player { Id = 291, FirstName = "Vitinha", LastName = "Ferreira", DateOfBirth = new DateTime(2000, 2, 13), Position = Position.Midfielder, ClubId = 32, Nationality = "Portugal" },
                new Player { Id = 292, FirstName = "João", LastName = "Neves", DateOfBirth = new DateTime(2004, 9, 27), Position = Position.Midfielder, ClubId = 32, Nationality = "Portugal" },
                new Player { Id = 293, FirstName = "Fabián", LastName = "Ruiz", DateOfBirth = new DateTime(1996, 4, 3), Position = Position.Midfielder, ClubId = 32, Nationality = "Spain" },
                new Player { Id = 294, FirstName = "Ousmane", LastName = "Dembélé", DateOfBirth = new DateTime(1997, 5, 15), Position = Position.Forward, ClubId = 32, Nationality = "France" },
                new Player { Id = 295, FirstName = "Khvicha", LastName = "Kvaratskhelia", DateOfBirth = new DateTime(2001, 2, 12), Position = Position.Forward, ClubId = 32, Nationality = "Georgia" },
                new Player { Id = 296, FirstName = "Bradley", LastName = "Barcola", DateOfBirth = new DateTime(2002, 9, 2), Position = Position.Forward, ClubId = 32, Nationality = "France" },
                new Player { Id = 297, FirstName = "Désiré", LastName = "Doué", DateOfBirth = new DateTime(2005, 6, 3), Position = Position.Forward, ClubId = 32, Nationality = "France" },
                // ── Marseille (33) ──
                new Player { Id = 298, FirstName = "Gerónimo", LastName = "Rulli", DateOfBirth = new DateTime(1992, 5, 20), Position = Position.Goalkeeper, ClubId = 33, Nationality = "Argentina" },
                new Player { Id = 299, FirstName = "Leonardo", LastName = "Balerdi", DateOfBirth = new DateTime(1999, 1, 26), Position = Position.Defender, ClubId = 33, Nationality = "Argentina" },
                new Player { Id = 300, FirstName = "Benjamin", LastName = "Pavard", DateOfBirth = new DateTime(1996, 3, 28), Position = Position.Defender, ClubId = 33, Nationality = "France" },
                new Player { Id = 301, FirstName = "Nayef", LastName = "Aguerd", DateOfBirth = new DateTime(1996, 3, 30), Position = Position.Defender, ClubId = 33, Nationality = "Morocco" },
                new Player { Id = 302, FirstName = "Pierre-Emile", LastName = "Højbjerg", DateOfBirth = new DateTime(1995, 8, 5), Position = Position.Midfielder, ClubId = 33, Nationality = "Denmark" },
                new Player { Id = 303, FirstName = "Geoffrey", LastName = "Kondogbia", DateOfBirth = new DateTime(1993, 2, 15), Position = Position.Midfielder, ClubId = 33, Nationality = "Central African Republic" },
                new Player { Id = 304, FirstName = "Mason", LastName = "Greenwood", DateOfBirth = new DateTime(2001, 10, 1), Position = Position.Forward, ClubId = 33, Nationality = "England" },
                new Player { Id = 305, FirstName = "Amine", LastName = "Gouiri", DateOfBirth = new DateTime(2000, 2, 16), Position = Position.Forward, ClubId = 33, Nationality = "Algeria" },
                new Player { Id = 306, FirstName = "Pierre-Emerick", LastName = "Aubameyang", DateOfBirth = new DateTime(1989, 6, 18), Position = Position.Forward, ClubId = 33, Nationality = "Gabon" },
                // ── Monaco (34) ──
                new Player { Id = 307, FirstName = "Philipp", LastName = "Köhn", DateOfBirth = new DateTime(1998, 4, 2), Position = Position.Goalkeeper, ClubId = 34, Nationality = "Switzerland" },
                new Player { Id = 308, FirstName = "Eric", LastName = "Dier", DateOfBirth = new DateTime(1994, 1, 15), Position = Position.Defender, ClubId = 34, Nationality = "England" },
                new Player { Id = 309, FirstName = "Thilo", LastName = "Kehrer", DateOfBirth = new DateTime(1996, 9, 21), Position = Position.Defender, ClubId = 34, Nationality = "Germany" },
                new Player { Id = 310, FirstName = "Vanderson", LastName = "de Oliveira", DateOfBirth = new DateTime(2001, 6, 21), Position = Position.Defender, ClubId = 34, Nationality = "Brazil" },
                new Player { Id = 311, FirstName = "Denis", LastName = "Zakaria", DateOfBirth = new DateTime(1996, 11, 20), Position = Position.Midfielder, ClubId = 34, Nationality = "Switzerland" },
                new Player { Id = 312, FirstName = "Paul", LastName = "Pogba", DateOfBirth = new DateTime(1993, 3, 15), Position = Position.Midfielder, ClubId = 34, Nationality = "France" },
                new Player { Id = 313, FirstName = "Aleksandr", LastName = "Golovin", DateOfBirth = new DateTime(1996, 5, 30), Position = Position.Midfielder, ClubId = 34, Nationality = "Russia" },
                new Player { Id = 314, FirstName = "Maghnes", LastName = "Akliouche", DateOfBirth = new DateTime(2002, 2, 25), Position = Position.Midfielder, ClubId = 34, Nationality = "France" },
                new Player { Id = 315, FirstName = "Folarin", LastName = "Balogun", DateOfBirth = new DateTime(2001, 7, 3), Position = Position.Forward, ClubId = 34, Nationality = "United States" },
                // ── Lyon (35) ──
                new Player { Id = 316, FirstName = "Dominik", LastName = "Greif", DateOfBirth = new DateTime(1997, 4, 6), Position = Position.Goalkeeper, ClubId = 35, Nationality = "Slovakia" },
                new Player { Id = 317, FirstName = "Nicolás", LastName = "Tagliafico", DateOfBirth = new DateTime(1992, 8, 31), Position = Position.Defender, ClubId = 35, Nationality = "Argentina" },
                new Player { Id = 318, FirstName = "Moussa", LastName = "Niakhaté", DateOfBirth = new DateTime(1996, 3, 8), Position = Position.Defender, ClubId = 35, Nationality = "Senegal" },
                new Player { Id = 319, FirstName = "Clinton", LastName = "Mata", DateOfBirth = new DateTime(1992, 11, 7), Position = Position.Defender, ClubId = 35, Nationality = "Angola" },
                new Player { Id = 320, FirstName = "Corentin", LastName = "Tolisso", DateOfBirth = new DateTime(1994, 8, 3), Position = Position.Midfielder, ClubId = 35, Nationality = "France" },
                new Player { Id = 321, FirstName = "Orel", LastName = "Mangala", DateOfBirth = new DateTime(1998, 3, 18), Position = Position.Midfielder, ClubId = 35, Nationality = "Belgium" },
                new Player { Id = 322, FirstName = "Georges", LastName = "Mikautadze", DateOfBirth = new DateTime(2000, 10, 31), Position = Position.Forward, ClubId = 35, Nationality = "Georgia" },
                new Player { Id = 323, FirstName = "Malick", LastName = "Fofana", DateOfBirth = new DateTime(2005, 3, 31), Position = Position.Forward, ClubId = 35, Nationality = "Belgium" },
                new Player { Id = 324, FirstName = "Endrick", LastName = "Felipe", DateOfBirth = new DateTime(2006, 7, 21), Position = Position.Forward, ClubId = 35, Nationality = "Brazil" },
                // ── Lille (36) ──
                new Player { Id = 325, FirstName = "Berke", LastName = "Özer", DateOfBirth = new DateTime(2000, 4, 10), Position = Position.Goalkeeper, ClubId = 36, Nationality = "Turkey" },
                new Player { Id = 326, FirstName = "Aïssa", LastName = "Mandi", DateOfBirth = new DateTime(1991, 10, 22), Position = Position.Defender, ClubId = 36, Nationality = "Algeria" },
                new Player { Id = 327, FirstName = "Thomas", LastName = "Meunier", DateOfBirth = new DateTime(1991, 9, 12), Position = Position.Defender, ClubId = 36, Nationality = "Belgium" },
                new Player { Id = 328, FirstName = "Chancel", LastName = "Mbemba", DateOfBirth = new DateTime(1994, 8, 8), Position = Position.Defender, ClubId = 36, Nationality = "DR Congo" },
                new Player { Id = 329, FirstName = "Benjamin", LastName = "André", DateOfBirth = new DateTime(1990, 8, 3), Position = Position.Midfielder, ClubId = 36, Nationality = "France" },
                new Player { Id = 330, FirstName = "Nabil", LastName = "Bentaleb", DateOfBirth = new DateTime(1994, 11, 24), Position = Position.Midfielder, ClubId = 36, Nationality = "Algeria" },
                new Player { Id = 331, FirstName = "Hákon", LastName = "Haraldsson", DateOfBirth = new DateTime(2003, 4, 10), Position = Position.Midfielder, ClubId = 36, Nationality = "Iceland" },
                new Player { Id = 332, FirstName = "Olivier", LastName = "Giroud", DateOfBirth = new DateTime(1986, 9, 30), Position = Position.Forward, ClubId = 36, Nationality = "France" },
                new Player { Id = 333, FirstName = "Ethan", LastName = "Mbappé", DateOfBirth = new DateTime(2007, 12, 29), Position = Position.Midfielder, ClubId = 36, Nationality = "France" },
                // ── Lens (37) ──
                new Player { Id = 334, FirstName = "Florian", LastName = "Sotoca", DateOfBirth = new DateTime(1990, 10, 19), Position = Position.Forward, ClubId = 37, Nationality = "France" },
                new Player { Id = 335, FirstName = "Florian", LastName = "Thauvin", DateOfBirth = new DateTime(1993, 1, 26), Position = Position.Forward, ClubId = 37, Nationality = "France" },
                new Player { Id = 336, FirstName = "Allan", LastName = "Saint-Maximin", DateOfBirth = new DateTime(1997, 3, 12), Position = Position.Forward, ClubId = 37, Nationality = "France" },
                new Player { Id = 337, FirstName = "Odsonne", LastName = "Édouard", DateOfBirth = new DateTime(1998, 1, 16), Position = Position.Forward, ClubId = 37, Nationality = "France" },
                new Player { Id = 338, FirstName = "Amadou", LastName = "Haidara", DateOfBirth = new DateTime(1998, 1, 31), Position = Position.Midfielder, ClubId = 37, Nationality = "Mali" },
                new Player { Id = 339, FirstName = "Jonathan", LastName = "Gradit", DateOfBirth = new DateTime(1992, 11, 24), Position = Position.Defender, ClubId = 37, Nationality = "France" },
                new Player { Id = 340, FirstName = "Ruben", LastName = "Aguilar", DateOfBirth = new DateTime(1993, 4, 26), Position = Position.Defender, ClubId = 37, Nationality = "France" },
                new Player { Id = 341, FirstName = "Abdallah", LastName = "Sima", DateOfBirth = new DateTime(2001, 6, 17), Position = Position.Forward, ClubId = 37, Nationality = "Senegal" });

            modelBuilder.Entity<Match>().HasData(
                // Premier League
                new Match { Id = 1, LeagueId = 1, HomeTeamId = 1, AwayTeamId = 2, Date = new DateTime(2025, 9, 21, 16, 30, 0), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 2, LeagueId = 1, HomeTeamId = 2, AwayTeamId = 1, Date = new DateTime(2026, 2, 22, 16, 30, 0), HomeGoals = 2, AwayGoals = 1 },
                new Match { Id = 3, LeagueId = 1, HomeTeamId = 5, AwayTeamId = 3, Date = new DateTime(2026, 1, 18, 16, 30, 0), HomeGoals = 1, AwayGoals = 2 },
                new Match { Id = 4, LeagueId = 1, HomeTeamId = 3, AwayTeamId = 5, Date = new DateTime(2025, 8, 24, 16, 30, 0), HomeGoals = 2, AwayGoals = 0 },
                new Match { Id = 5, LeagueId = 1, HomeTeamId = 1, AwayTeamId = 8, Date = new DateTime(2025, 11, 23, 14, 0, 0), HomeGoals = 4, AwayGoals = 1 },
                new Match { Id = 6, LeagueId = 1, HomeTeamId = 6, AwayTeamId = 1, Date = new DateTime(2025, 11, 30, 16, 30, 0), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 7, LeagueId = 1, HomeTeamId = 2, AwayTeamId = 5, Date = new DateTime(2025, 11, 9, 16, 30, 0), HomeGoals = 3, AwayGoals = 0 },
                new Match { Id = 8, LeagueId = 1, HomeTeamId = 7, AwayTeamId = 1, Date = new DateTime(2025, 9, 28, 14, 0, 0), HomeGoals = 1, AwayGoals = 2 },
                new Match { Id = 9, LeagueId = 1, HomeTeamId = 4, AwayTeamId = 2, Date = new DateTime(2026, 1, 25, 14, 0, 0), HomeGoals = 1, AwayGoals = 0 },
                // La Liga
                new Match { Id = 10, LeagueId = 2, HomeTeamId = 10, AwayTeamId = 9, Date = new DateTime(2025, 10, 26, 21, 0, 0), HomeGoals = 2, AwayGoals = 1 },
                new Match { Id = 11, LeagueId = 2, HomeTeamId = 9, AwayTeamId = 10, Date = new DateTime(2026, 5, 10, 21, 0, 0), HomeGoals = 2, AwayGoals = 0 },
                new Match { Id = 12, LeagueId = 2, HomeTeamId = 9, AwayTeamId = 11, Date = new DateTime(2025, 12, 21, 21, 0, 0), HomeGoals = 3, AwayGoals = 1 },
                new Match { Id = 13, LeagueId = 2, HomeTeamId = 16, AwayTeamId = 13, Date = new DateTime(2026, 3, 8, 18, 30, 0), HomeGoals = 0, AwayGoals = 2 },
                new Match { Id = 14, LeagueId = 2, HomeTeamId = 15, AwayTeamId = 14, Date = new DateTime(2025, 11, 2, 18, 30, 0), HomeGoals = 3, AwayGoals = 2 },
                new Match { Id = 15, LeagueId = 2, HomeTeamId = 12, AwayTeamId = 9, Date = new DateTime(2026, 2, 15, 21, 0, 0), HomeGoals = 0, AwayGoals = 2 },
                // Serie A
                new Match { Id = 16, LeagueId = 3, HomeTeamId = 17, AwayTeamId = 18, Date = new DateTime(2025, 10, 19, 20, 45, 0), HomeGoals = 0, AwayGoals = 1 },
                new Match { Id = 17, LeagueId = 3, HomeTeamId = 18, AwayTeamId = 17, Date = new DateTime(2026, 2, 1, 20, 45, 0), HomeGoals = 1, AwayGoals = 0 },
                new Match { Id = 18, LeagueId = 3, HomeTeamId = 19, AwayTeamId = 17, Date = new DateTime(2025, 9, 13, 18, 0, 0), HomeGoals = 4, AwayGoals = 3 },
                new Match { Id = 19, LeagueId = 3, HomeTeamId = 20, AwayTeamId = 17, Date = new DateTime(2025, 11, 30, 20, 45, 0), HomeGoals = 3, AwayGoals = 1 },
                new Match { Id = 20, LeagueId = 3, HomeTeamId = 18, AwayTeamId = 19, Date = new DateTime(2025, 11, 23, 20, 45, 0), HomeGoals = 0, AwayGoals = 0 },
                new Match { Id = 21, LeagueId = 3, HomeTeamId = 21, AwayTeamId = 23, Date = new DateTime(2026, 1, 11, 20, 45, 0), HomeGoals = 2, AwayGoals = 0 },
                new Match { Id = 22, LeagueId = 3, HomeTeamId = 17, AwayTeamId = 20, Date = new DateTime(2026, 3, 1, 20, 45, 0), HomeGoals = 2, AwayGoals = 2 },
                new Match { Id = 23, LeagueId = 3, HomeTeamId = 19, AwayTeamId = 18, Date = new DateTime(2026, 4, 5, 20, 45, 0), HomeGoals = 0, AwayGoals = 0 },
                // Bundesliga
                new Match { Id = 24, LeagueId = 4, HomeTeamId = 26, AwayTeamId = 25, Date = new DateTime(2026, 4, 4, 18, 30, 0), HomeGoals = 2, AwayGoals = 1 },
                new Match { Id = 25, LeagueId = 4, HomeTeamId = 25, AwayTeamId = 27, Date = new DateTime(2025, 9, 20, 18, 30, 0), HomeGoals = 6, AwayGoals = 0 },
                new Match { Id = 26, LeagueId = 4, HomeTeamId = 28, AwayTeamId = 25, Date = new DateTime(2026, 2, 7, 18, 30, 0), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 27, LeagueId = 4, HomeTeamId = 26, AwayTeamId = 27, Date = new DateTime(2025, 10, 25, 15, 30, 0), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 28, LeagueId = 4, HomeTeamId = 27, AwayTeamId = 26, Date = new DateTime(2026, 3, 14, 15, 30, 0), HomeGoals = 1, AwayGoals = 5 },
                // Ligue 1
                new Match { Id = 29, LeagueId = 5, HomeTeamId = 32, AwayTeamId = 33, Date = new DateTime(2025, 9, 21, 20, 45, 0), HomeGoals = 5, AwayGoals = 0 },
                new Match { Id = 30, LeagueId = 5, HomeTeamId = 33, AwayTeamId = 32, Date = new DateTime(2026, 3, 15, 20, 45, 0), HomeGoals = 1, AwayGoals = 0 },
                new Match { Id = 31, LeagueId = 5, HomeTeamId = 32, AwayTeamId = 34, Date = new DateTime(2025, 12, 14, 20, 45, 0), HomeGoals = 1, AwayGoals = 3 },
                new Match { Id = 32, LeagueId = 5, HomeTeamId = 33, AwayTeamId = 35, Date = new DateTime(2025, 11, 8, 20, 0, 0), HomeGoals = 3, AwayGoals = 2 },
                new Match { Id = 33, LeagueId = 5, HomeTeamId = 36, AwayTeamId = 32, Date = new DateTime(2026, 1, 31, 20, 0, 0), HomeGoals = 1, AwayGoals = 1 },
                new Match { Id = 34, LeagueId = 5, HomeTeamId = 32, AwayTeamId = 35, Date = new DateTime(2026, 2, 8, 20, 45, 0), HomeGoals = 1, AwayGoals = 2 });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Liam", LastName = "O'Connor", Email = "liam.oconnor@futscores.app" },
                new User { Id = 2, FirstName = "Sofia", LastName = "Marchetti", Email = "sofia.marchetti@futscores.app" },
                new User { Id = 3, FirstName = "Mateo", LastName = "Fernández", Email = "mateo.fernandez@futscores.app" },
                new User { Id = 4, FirstName = "Emma", LastName = "Schneider", Email = "emma.schneider@futscores.app" },
                new User { Id = 5, FirstName = "Hugo", LastName = "Lefèvre", Email = "hugo.lefevre@futscores.app" },
                new User { Id = 6, FirstName = "Olivia", LastName = "Bennett", Email = "olivia.bennett@futscores.app" },
                new User { Id = 7, FirstName = "Noah", LastName = "Andersson", Email = "noah.andersson@futscores.app" },
                new User { Id = 8, FirstName = "Chiara", LastName = "Romano", Email = "chiara.romano@futscores.app" },
                new User { Id = 9, FirstName = "Daniel", LastName = "Kovač", Email = "daniel.kovac@futscores.app" },
                new User { Id = 10, FirstName = "Aisha", LastName = "Rahman", Email = "aisha.rahman@futscores.app" });

            modelBuilder.Entity<Rating>().HasData(
                // Match 5 — Arsenal 4–1 Tottenham
                new Rating { Id = 1, PlayerId = 7, MatchId = 5, UserId = 1, Score = 9, Comment = "Unplayable on the right — two goals and a constant threat." },
                new Rating { Id = 2, PlayerId = 5, MatchId = 5, UserId = 3, Score = 8, Comment = "Pulled the strings and capped it with a classy assist." },
                new Rating { Id = 3, PlayerId = 9, MatchId = 5, UserId = 2, Score = 8, Comment = "Powerful display through the middle, took his goal well." },
                new Rating { Id = 4, PlayerId = 72, MatchId = 5, UserId = 4, Score = 5, Comment = "A few bright moments but couldn't drag Spurs back in." },
                // Match 1 — Arsenal 1–1 Manchester City
                new Rating { Id = 5, PlayerId = 4, MatchId = 1, UserId = 5, Score = 8, Comment = "Bossed the midfield battle against City." },
                new Rating { Id = 6, PlayerId = 18, MatchId = 1, UserId = 1, Score = 7, Comment = "Took his goal but starved of service otherwise." },
                new Rating { Id = 7, PlayerId = 2, MatchId = 1, UserId = 6, Score = 8, Comment = "Marshalled Haaland superbly all afternoon." },
                // Match 2 — Manchester City 2–1 Arsenal
                new Rating { Id = 8, PlayerId = 18, MatchId = 2, UserId = 7, Score = 9, Comment = "Decisive brace — back to his ruthless best." },
                new Rating { Id = 9, PlayerId = 15, MatchId = 2, UserId = 2, Score = 8, Comment = "Drove City forward relentlessly." },
                new Rating { Id = 10, PlayerId = 7, MatchId = 2, UserId = 8, Score = 7, Comment = "Scored but was largely kept quiet." },
                // Match 3 — Liverpool 1–2 Manchester United
                new Rating { Id = 11, PlayerId = 26, MatchId = 3, UserId = 1, Score = 9, Comment = "Two goals at Anfield — utterly ruthless." },
                new Rating { Id = 12, PlayerId = 23, MatchId = 3, UserId = 2, Score = 8, Comment = "Ran the show for United from midfield." },
                new Rating { Id = 13, PlayerId = 46, MatchId = 3, UserId = 3, Score = 7, Comment = "Scored but lacked support up top." },
                // Match 4 — Manchester United 2–0 Liverpool
                new Rating { Id = 14, PlayerId = 28, MatchId = 4, UserId = 4, Score = 8, Comment = "Strong hold-up play crowned with a goal." },
                new Rating { Id = 15, PlayerId = 27, MatchId = 4, UserId = 5, Score = 8, Comment = "Tireless running, thoroughly deserved his goal." },
                new Rating { Id = 16, PlayerId = 39, MatchId = 4, UserId = 6, Score = 5, Comment = "An off night for the skipper." },
                // Match 7 — Manchester City 3–0 Liverpool
                new Rating { Id = 17, PlayerId = 18, MatchId = 7, UserId = 9, Score = 10, Comment = "Hat-trick hero — simply unstoppable." },
                new Rating { Id = 18, PlayerId = 19, MatchId = 7, UserId = 10, Score = 8, Comment = "Terrorised the full-back all game." },
                // Match 8 — Newcastle 1–2 Arsenal
                new Rating { Id = 19, PlayerId = 8, MatchId = 8, UserId = 1, Score = 8, Comment = "His pace cut Newcastle open repeatedly." },
                new Rating { Id = 20, PlayerId = 63, MatchId = 8, UserId = 2, Score = 6, Comment = "Worked hard but little end product." },
                // Match 9 — Aston Villa 1–0 Manchester City
                new Rating { Id = 21, PlayerId = 36, MatchId = 9, UserId = 3, Score = 9, Comment = "Match-winning goal and led the line superbly." },
                new Rating { Id = 22, PlayerId = 29, MatchId = 9, UserId = 4, Score = 9, Comment = "A wall in goal — kept City out single-handedly." },
                // Match 10 — Real Madrid 2–1 Barcelona
                new Rating { Id = 23, PlayerId = 93, MatchId = 10, UserId = 5, Score = 9, Comment = "Decisive brace in the Clásico." },
                new Rating { Id = 24, PlayerId = 91, MatchId = 10, UserId = 6, Score = 8, Comment = "Bossed the midfield in the biggest game." },
                new Rating { Id = 25, PlayerId = 83, MatchId = 10, UserId = 7, Score = 8, Comment = "Scored a beauty to keep Barça in it." },
                // Match 11 — Barcelona 2–0 Real Madrid
                new Rating { Id = 26, PlayerId = 84, MatchId = 11, UserId = 8, Score = 8, Comment = "Clinical finish and led the press." },
                new Rating { Id = 27, PlayerId = 79, MatchId = 11, UserId = 9, Score = 9, Comment = "Conducted the win — flawless tempo." },
                new Rating { Id = 28, PlayerId = 94, MatchId = 11, UserId = 10, Score = 5, Comment = "Frustrating night, well shackled by the back line." },
                // Match 12 — Barcelona 3–1 Atlético
                new Rating { Id = 29, PlayerId = 82, MatchId = 12, UserId = 1, Score = 9, Comment = "Two goals and relentless from the left." },
                new Rating { Id = 30, PlayerId = 104, MatchId = 12, UserId = 2, Score = 7, Comment = "Battled hard and took his goal." },
                // Match 13 — Sevilla 0–2 Real Betis
                new Rating { Id = 31, PlayerId = 119, MatchId = 13, UserId = 3, Score = 9, Comment = "Stole the derby with a stunning strike." },
                new Rating { Id = 32, PlayerId = 116, MatchId = 13, UserId = 4, Score = 8, Comment = "A masterclass in the number ten role." },
                // Match 14 — Real Sociedad 3–2 Athletic Bilbao
                new Rating { Id = 33, PlayerId = 136, MatchId = 14, UserId = 5, Score = 9, Comment = "Captain's brace in a thrilling Basque derby." },
                new Rating { Id = 34, PlayerId = 128, MatchId = 14, UserId = 6, Score = 7, Comment = "Lively throughout and scored a fine goal." },
                // Match 15 — Villarreal 0–2 Barcelona
                new Rating { Id = 35, PlayerId = 83, MatchId = 15, UserId = 7, Score = 8, Comment = "A wonderful solo goal sealed it." },
                new Rating { Id = 36, PlayerId = 107, MatchId = 15, UserId = 8, Score = 5, Comment = "Couldn't contain Barça's movement." },
                // Match 16 — Inter 0–1 AC Milan
                new Rating { Id = 37, PlayerId = 163, MatchId = 16, UserId = 9, Score = 9, Comment = "Won the derby with a brilliant solo goal." },
                new Rating { Id = 38, PlayerId = 160, MatchId = 16, UserId = 10, Score = 8, Comment = "Controlled the tempo with ease at his age." },
                new Rating { Id = 39, PlayerId = 153, MatchId = 16, UserId = 1, Score = 5, Comment = "Quiet by his lofty standards." },
                // Match 18 — Juventus 4–3 Inter
                new Rating { Id = 40, PlayerId = 171, MatchId = 18, UserId = 2, Score = 9, Comment = "Sensational in a seven-goal thriller." },
                new Rating { Id = 41, PlayerId = 153, MatchId = 18, UserId = 3, Score = 8, Comment = "Scored twice but ended on the losing side." },
                // Match 19 — Napoli 3–1 Inter
                new Rating { Id = 42, PlayerId = 180, MatchId = 19, UserId = 4, Score = 9, Comment = "Pulled the strings with two assists." },
                new Rating { Id = 43, PlayerId = 181, MatchId = 19, UserId = 5, Score = 8, Comment = "Bullied his former club all night." },
                // Match 21 — Roma 2–0 Lazio
                new Rating { Id = 44, PlayerId = 189, MatchId = 21, UserId = 6, Score = 9, Comment = "Derby della Capitale hero." },
                new Rating { Id = 45, PlayerId = 190, MatchId = 21, UserId = 7, Score = 8, Comment = "Direct and dangerous every time he ran." },
                // Match 22 — Inter 2–2 Napoli
                new Rating { Id = 46, PlayerId = 152, MatchId = 22, UserId = 8, Score = 8, Comment = "Two assists and ran the midfield." },
                new Rating { Id = 47, PlayerId = 179, MatchId = 22, UserId = 9, Score = 8, Comment = "A relentless box-to-box engine." },
                // Match 24 — Dortmund 2–1 Bayern
                new Rating { Id = 48, PlayerId = 237, MatchId = 24, UserId = 10, Score = 9, Comment = "Decisive brace in Der Klassiker." },
                new Rating { Id = 49, PlayerId = 227, MatchId = 24, UserId = 1, Score = 7, Comment = "Scored but Bayern fell short." },
                // Match 25 — Bayern 6–0 RB Leipzig
                new Rating { Id = 50, PlayerId = 227, MatchId = 25, UserId = 2, Score = 9, Comment = "Hat-trick in a ruthless rout." },
                new Rating { Id = 51, PlayerId = 226, MatchId = 25, UserId = 3, Score = 9, Comment = "Three assists — utterly unplayable." },
                new Rating { Id = 52, PlayerId = 225, MatchId = 25, UserId = 4, Score = 8, Comment = "Dazzling on the ball throughout." },
                // Match 28 — RB Leipzig 1–5 Dortmund
                new Rating { Id = 53, PlayerId = 236, MatchId = 28, UserId = 5, Score = 9, Comment = "His pace destroyed Leipzig." },
                new Rating { Id = 54, PlayerId = 237, MatchId = 28, UserId = 6, Score = 8, Comment = "Two more goals for the in-form striker." },
                // Match 29 — PSG 5–0 Marseille
                new Rating { Id = 55, PlayerId = 294, MatchId = 29, UserId = 7, Score = 9, Comment = "Ballon d'Or form — a brace in Le Classique." },
                new Rating { Id = 56, PlayerId = 295, MatchId = 29, UserId = 8, Score = 9, Comment = "Unstoppable: a goal and two assists." },
                new Rating { Id = 57, PlayerId = 296, MatchId = 29, UserId = 9, Score = 8, Comment = "Electric pace down the flank." },
                // Match 30 — Marseille 1–0 PSG
                new Rating { Id = 58, PlayerId = 306, MatchId = 30, UserId = 10, Score = 9, Comment = "The winner against the champions." },
                new Rating { Id = 59, PlayerId = 302, MatchId = 30, UserId = 1, Score = 8, Comment = "Immense in midfield, never gave the ball away." },
                // Match 31 — PSG 1–3 Monaco
                new Rating { Id = 60, PlayerId = 315, MatchId = 31, UserId = 2, Score = 9, Comment = "A clinical hat-trick at the Parc des Princes." },
                new Rating { Id = 61, PlayerId = 312, MatchId = 31, UserId = 3, Score = 8, Comment = "Turned back the clock with a commanding display." },
                // Match 32 — Marseille 3–2 Lyon
                new Rating { Id = 62, PlayerId = 304, MatchId = 32, UserId = 4, Score = 9, Comment = "Two goals in a five-goal thriller." },
                new Rating { Id = 63, PlayerId = 322, MatchId = 32, UserId = 5, Score = 8, Comment = "A brace for Lyon in a narrow defeat." },
                // Match 34 — PSG 1–2 Lyon
                new Rating { Id = 64, PlayerId = 324, MatchId = 34, UserId = 6, Score = 9, Comment = "The match-winner at the Parc — a star turn." },
                new Rating { Id = 65, PlayerId = 291, MatchId = 34, UserId = 7, Score = 7, Comment = "Tried everything to rescue PSG." });
        }
    }
}
