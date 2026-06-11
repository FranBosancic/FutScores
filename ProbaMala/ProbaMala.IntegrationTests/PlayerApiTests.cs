using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za PlayerApiController. Player ima FK na Club
    // (pa seed lanac ide Liga → Club → Player), Position enum koji DTO vraća kao string,
    // i nema pravilo jedinstvenosti imena. Svaki test dobije svoj factory → svježu bazu.
    public class PlayerApiTests : IDisposable
    {
        private static readonly DateTime SampleDob = new(2000, 1, 1);

        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;        // neautentificiran
        private readonly HttpClient _adminClient;   // admin rola

        public PlayerApiTests()
        {
            _factory     = new FutScoresApiFactory();
            _client      = _factory.CreateClient();
            _adminClient = _factory.CreateAdminClient();
        }

        public void Dispose()
        {
            _adminClient.Dispose();
            _client.Dispose();
            _factory.Dispose();
        }

        private async Task<Club> SeedClubAsync(string leagueName, string clubName)
        {
            var league = new League { Name = leagueName };
            var club   = new Club { Name = clubName, FoundedDate = new DateTime(1900, 1, 1), League = league };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Clubs.Add(club);
                await db.SaveChangesAsync();
            });
            return club;
        }

        private async Task<Player> SeedPlayerAsync(
            int clubId, string firstName, string lastName,
            Position position = Position.Midfielder, string nationality = "Croatia")
        {
            var player = new Player
            {
                FirstName   = firstName,
                LastName    = lastName,
                DateOfBirth = SampleDob,
                Position    = position,
                Nationality = nationality,
                ClubId      = clubId
            };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Players.Add(player);
                await db.SaveChangesAsync();
            });
            return player;
        }

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllPlayers_OrderedByLastNameThenFirstName()
        {
            var club = await SeedClubAsync("Premier League", "Test FC");
            await SeedPlayerAsync(club.Id, "Ana",   "Zoric");
            await SeedPlayerAsync(club.Id, "Petar", "Anic");
            await SeedPlayerAsync(club.Id, "Marko", "Anic");

            var response = await _client.GetAsync("/api/players");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var players = await response.Content.ReadFromJsonAsync<List<PlayerDTO>>();
            players.Should().NotBeNull();
            players!.Select(p => $"{p.LastName} {p.FirstName}")
                .Should().Equal("Anic Marko", "Anic Petar", "Zoric Ana");
            players.Should().OnlyContain(p => p.Club.Name == "Test FC");
        }

        [Fact]
        public async Task GetAll_FiltersByClubId_WhenProvided()
        {
            var first  = await SeedClubAsync("League A", "First FC");
            var second = await SeedClubAsync("League B", "Second FC");
            await SeedPlayerAsync(first.Id,  "One", "Player");
            await SeedPlayerAsync(second.Id, "Two", "Player");

            var response = await _client.GetAsync($"/api/players?clubId={first.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var players = await response.Content.ReadFromJsonAsync<List<PlayerDTO>>();
            players.Should().ContainSingle();
            players![0].FirstName.Should().Be("One");
            players[0].Club.Id.Should().Be(first.Id);
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsPlayer_WhenPlayerExists()
        {
            var club   = await SeedClubAsync("La Liga", "Some FC");
            var player = await SeedPlayerAsync(club.Id, "John", "Doe", Position.Forward, "England");

            var response = await _client.GetAsync($"/api/players/{player.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<PlayerDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(player.Id);
            dto.FirstName.Should().Be("John");
            dto.LastName.Should().Be("Doe");
            dto.DateOfBirth.Should().Be(SampleDob);
            dto.Position.Should().Be("Forward");
            dto.Nationality.Should().Be("England");
            dto.Club.Id.Should().Be(club.Id);
            dto.Club.Name.Should().Be("Some FC");
        }

        [Fact]
        public async Task GetById_Returns404_WhenPlayerMissing()
        {
            var response = await _client.GetAsync("/api/players/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesPlayer_AndReturns201()
        {
            var club = await SeedClubAsync("Serie A", "Create FC");

            var response = await _adminClient.PostAsJsonAsync("/api/players", new
            {
                firstName   = "New",
                lastName    = "Player",
                dateOfBirth = SampleDob,
                position    = Position.Defender,
                nationality = "Italy",
                clubId      = club.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<PlayerDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.FirstName.Should().Be("New");
            dto.Position.Should().Be("Defender");
            dto.Club.Id.Should().Be(club.Id);

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Players.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.ClubId.Should().Be(club.Id);
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenClubDoesNotExist()
        {
            var response = await _adminClient.PostAsJsonAsync("/api/players", new
            {
                firstName   = "Orphan",
                lastName    = "Player",
                dateOfBirth = SampleDob,
                position    = Position.Goalkeeper,
                nationality = "Spain",
                clubId      = 999999
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenRequiredFieldMissing()
        {
            var club = await SeedClubAsync("Bundesliga", "Valid FC");

            var response = await _adminClient.PostAsJsonAsync("/api/players", new
            {
                firstName   = "",
                lastName    = "Player",
                dateOfBirth = SampleDob,
                position    = Position.Midfielder,
                nationality = "Germany",
                clubId      = club.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesPlayer_AndReturns200()
        {
            var club   = await SeedClubAsync("Ligue 1", "Update FC");
            var player = await SeedPlayerAsync(club.Id, "Old", "Name");

            var response = await _adminClient.PutAsJsonAsync($"/api/players/{player.Id}", new
            {
                firstName   = "New",
                lastName    = "Name",
                dateOfBirth = SampleDob,
                position    = Position.Forward,
                nationality = "France",
                clubId      = club.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<PlayerDTO>();
            dto.Should().NotBeNull();
            dto!.FirstName.Should().Be("New");
            dto.Position.Should().Be("Forward");

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Players.FindAsync(player.Id);
                updated!.FirstName.Should().Be("New");
                updated.Position.Should().Be(Position.Forward);
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenPlayerMissing()
        {
            var club = await SeedClubAsync("Eredivisie", "Ghost FC");

            var response = await _adminClient.PutAsJsonAsync("/api/players/999999", new
            {
                firstName   = "Ghost",
                lastName    = "Player",
                dateOfBirth = SampleDob,
                position    = Position.Midfielder,
                nationality = "Netherlands",
                clubId      = club.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Returns400_WhenClubDoesNotExist()
        {
            var club   = await SeedClubAsync("Primeira Liga", "Real FC");
            var player = await SeedPlayerAsync(club.Id, "Stay", "Here");

            var response = await _adminClient.PutAsJsonAsync($"/api/players/{player.Id}", new
            {
                firstName   = "Stay",
                lastName    = "Here",
                dateOfBirth = SampleDob,
                position    = Position.Midfielder,
                nationality = "Portugal",
                clubId      = 999999
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesPlayer_AndReturns204()
        {
            var club   = await SeedClubAsync("Scottish Premiership", "Delete FC");
            var player = await SeedPlayerAsync(club.Id, "To", "Delete");

            var response = await _adminClient.DeleteAsync($"/api/players/{player.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Players.FindAsync(player.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenPlayerMissing()
        {
            var response = await _adminClient.DeleteAsync("/api/players/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── AUTORIZACIJA ───────────────────────────

        [Fact]
        public async Task Post_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PostAsJsonAsync("/api/players", new { firstName = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_Returns403_WhenRegularUser()
        {
            // Kreiranje igrača je samo za admina (kao na webu).
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PostAsJsonAsync("/api/players", new { firstName = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Put_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PutAsJsonAsync("/api/players/1", new { firstName = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PutAsJsonAsync("/api/players/1", new { firstName = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.DeleteAsync("/api/players/1");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.DeleteAsync("/api/players/1");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
