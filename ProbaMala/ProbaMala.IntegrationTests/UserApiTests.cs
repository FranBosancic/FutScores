using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za UserApiController (domenski User — autor
    // recenzija, nije AppUser). Ravni DTO, jedinstveni email (case-insensitive),
    // [EmailAddress] validacija i blokada brisanja ako korisnik ima recenzije.
    // Svaki test dobije svoj factory → svježu, izoliranu bazu.
    public class UserApiTests : IDisposable
    {
        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;

        public UserApiTests()
        {
            _factory = new FutScoresApiFactory();
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        private async Task<User> SeedUserAsync(string firstName, string lastName, string email)
        {
            var user = new User { FirstName = firstName, LastName = lastName, Email = email };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
            });
            return user;
        }

        // Za scenarij brisanja: korisnik koji ima jednu recenziju (treba cijeli graf).
        private async Task<User> SeedUserWithRatingAsync()
        {
            var league = new League { Name = "Premier League" };
            var home = new Club { Name = "Home FC", FoundedDate = new DateTime(1900, 1, 1), League = league };
            var away = new Club { Name = "Away FC", FoundedDate = new DateTime(1900, 1, 1), League = league };
            var player = new Player
            {
                FirstName = "Test",
                LastName = "Player",
                DateOfBirth = new DateTime(2000, 1, 1),
                Position = Position.Forward,
                Nationality = "Croatia",
                Club = home
            };
            var match = new Match
            {
                League = league,
                HomeTeam = home,
                AwayTeam = away,
                Date = new DateTime(2024, 1, 1),
                HomeGoals = 1,
                AwayGoals = 0
            };
            var user = new User { FirstName = "Rated", LastName = "User", Email = "rated@example.com" };
            var rating = new Rating { Player = player, Match = match, User = user, Score = 7 };

            await _factory.WithDbContextAsync(async db =>
            {
                // Dodavanje ratinga povlači cijeli graf (player/match/user/liga/klubovi).
                db.Ratings.Add(rating);
                await db.SaveChangesAsync();
            });
            return user;
        }

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllUsers_OrderedByLastNameThenFirstName()
        {
            // Arrange
            await SeedUserAsync("Ana", "Zoric", "ana@example.com");
            await SeedUserAsync("Petar", "Anic", "petar@example.com");
            await SeedUserAsync("Marko", "Anic", "marko@example.com");

            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert — 200 + poredak po prezimenu pa imenu.
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            users.Should().NotBeNull();
            users!.Select(u => $"{u.LastName} {u.FirstName}")
                .Should().Equal("Anic Marko", "Anic Petar", "Zoric Ana");
        }

        [Fact]
        public async Task GetAll_FiltersByQuery_WhenQProvided()
        {
            // Arrange — različite domene emaila.
            await SeedUserAsync("John", "Smith", "john@example.com");
            await SeedUserAsync("Jane", "Doe", "jane@test.com");

            // Act — pretraga po emailu.
            var response = await _client.GetAsync("/api/users?q=example");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var users = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            users.Should().ContainSingle();
            users![0].Email.Should().Be("john@example.com");
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = await SeedUserAsync("Test", "User", "test@example.com");

            // Act
            var response = await _client.GetAsync($"/api/users/{user.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(user.Id);
            dto.FirstName.Should().Be("Test");
            dto.LastName.Should().Be("User");
            dto.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetById_Returns404_WhenUserMissing()
        {
            var response = await _client.GetAsync("/api/users/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesUser_AndReturns201()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/users", new
            {
                firstName = "New",
                lastName = "User",
                email = "new@example.com"
            });

            // Assert — 201 + Location + DTO, i zapis u bazi.
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.Email.Should().Be("new@example.com");

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Users.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.Email.Should().Be("new@example.com");
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenEmailAlreadyExists()
        {
            // Arrange — korisnik s tim emailom već postoji.
            await SeedUserAsync("Existing", "User", "existing@example.com");

            // Act — isti email, drugačiji case (provjera case-insensitive pravila).
            var response = await _client.PostAsJsonAsync("/api/users", new
            {
                firstName = "Another",
                lastName = "Person",
                email = "Existing@Example.com"
            });

            // Assert — 400 i nije nastao drugi zapis.
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await _factory.WithDbContextAsync(async db =>
            {
                var count = await db.Users.CountAsync();
                count.Should().Be(1);
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenEmailInvalid()
        {
            // Act — nevažeći format emaila pada na [EmailAddress] validaciji.
            var response = await _client.PostAsJsonAsync("/api/users", new
            {
                firstName = "Bad",
                lastName = "Email",
                email = "notanemail"
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesUser_AndReturns200()
        {
            // Arrange
            var user = await SeedUserAsync("Old", "Name", "old@example.com");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{user.Id}", new
            {
                firstName = "New",
                lastName = "Name",
                email = "newemail@example.com"
            });

            // Assert — 200 + DTO s novim vrijednostima, i promjena u bazi.
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<UserDTO>();
            dto.Should().NotBeNull();
            dto!.FirstName.Should().Be("New");
            dto.Email.Should().Be("newemail@example.com");

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Users.FindAsync(user.Id);
                updated!.Email.Should().Be("newemail@example.com");
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenUserMissing()
        {
            // Act — valjan model, ali korisnik ne postoji.
            var response = await _client.PutAsJsonAsync("/api/users/999999", new
            {
                firstName = "Ghost",
                lastName = "User",
                email = "ghost@example.com"
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Returns400_WhenEmailBelongsToAnotherUser()
        {
            // Arrange — dva korisnika; pokušavamo prvom dati email drugog.
            var first = await SeedUserAsync("First", "User", "first@example.com");
            await SeedUserAsync("Second", "User", "second@example.com");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{first.Id}", new
            {
                firstName = "First",
                lastName = "User",
                email = "second@example.com"
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesUser_AndReturns204()
        {
            // Arrange — korisnik bez recenzija (smije se obrisati).
            var user = await SeedUserAsync("To", "Delete", "delete@example.com");

            // Act
            var response = await _client.DeleteAsync($"/api/users/{user.Id}");

            // Assert — 204 i zapisa više nema.
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Users.FindAsync(user.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenUserMissing()
        {
            var response = await _client.DeleteAsync("/api/users/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_Returns400_WhenUserHasRatings()
        {
            // Arrange — korisnik koji ima recenziju → brisanje je blokirano.
            var user = await SeedUserWithRatingAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/users/{user.Id}");

            // Assert — 400 i korisnik je i dalje u bazi.
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await _factory.WithDbContextAsync(async db =>
            {
                var stillThere = await db.Users.FindAsync(user.Id);
                stillThere.Should().NotBeNull();
            });
        }
    }
}
