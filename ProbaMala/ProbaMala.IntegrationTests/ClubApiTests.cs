using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za ClubApiController. Club ima ugniježđeni
    // LeagueSummaryDTO i FK na ligu, pa seed helperi prvo kreiraju ligu. Svaki test
    // dobije svoj factory → svježu, izoliranu in-memory bazu.
    public class ClubApiTests : IDisposable
    {
        private static readonly DateTime SampleFoundedDate = new(1990, 5, 15);

        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;        // neautentificiran
        private readonly HttpClient _adminClient;   // admin rola

        public ClubApiTests()
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

        private async Task<League> SeedLeagueAsync(string name)
        {
            var league = new League { Name = name };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Leagues.Add(league);
                await db.SaveChangesAsync();
            });
            return league;
        }

        private async Task<Club> SeedClubAsync(int leagueId, string name)
        {
            var club = new Club { Name = name, FoundedDate = SampleFoundedDate, LeagueId = leagueId };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Clubs.Add(club);
                await db.SaveChangesAsync();
            });
            return club;
        }

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllClubs_OrderedByName()
        {
            var league = await SeedLeagueAsync("Premier League");
            await SeedClubAsync(league.Id, "Zeta FC");
            await SeedClubAsync(league.Id, "Alpha FC");
            await SeedClubAsync(league.Id, "Mid FC");

            var response = await _client.GetAsync("/api/clubs");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var clubs = await response.Content.ReadFromJsonAsync<List<ClubDTO>>();
            clubs.Should().NotBeNull();
            clubs!.Select(c => c.Name).Should().Equal("Alpha FC", "Mid FC", "Zeta FC");
            clubs.Should().OnlyContain(c => c.League.Name == "Premier League");
        }

        [Fact]
        public async Task GetAll_FiltersByLeagueId_WhenProvided()
        {
            var first  = await SeedLeagueAsync("First League");
            var second = await SeedLeagueAsync("Second League");
            await SeedClubAsync(first.Id, "First Club");
            await SeedClubAsync(second.Id, "Second Club");

            var response = await _client.GetAsync($"/api/clubs?leagueId={first.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var clubs = await response.Content.ReadFromJsonAsync<List<ClubDTO>>();
            clubs.Should().ContainSingle();
            clubs![0].Name.Should().Be("First Club");
            clubs[0].League.Id.Should().Be(first.Id);
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsClub_WhenClubExists()
        {
            var league = await SeedLeagueAsync("La Liga");
            var club   = await SeedClubAsync(league.Id, "Test Club");

            var response = await _client.GetAsync($"/api/clubs/{club.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ClubDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(club.Id);
            dto.Name.Should().Be("Test Club");
            dto.FoundedDate.Should().Be(SampleFoundedDate);
            dto.League.Id.Should().Be(league.Id);
            dto.League.Name.Should().Be("La Liga");
            dto.PlayerCount.Should().Be(0);
            dto.MatchCount.Should().Be(0);
        }

        [Fact]
        public async Task GetById_Returns404_WhenClubMissing()
        {
            var response = await _client.GetAsync("/api/clubs/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesClub_AndReturns201()
        {
            var league = await SeedLeagueAsync("Serie A");

            var response = await _adminClient.PostAsJsonAsync("/api/clubs", new
            {
                name = "New Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<ClubDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.Name.Should().Be("New Club");
            dto.League.Id.Should().Be(league.Id);

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Clubs.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.LeagueId.Should().Be(league.Id);
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenLeagueDoesNotExist()
        {
            var response = await _adminClient.PostAsJsonAsync("/api/clubs", new
            {
                name = "Orphan Club",
                foundedDate = SampleFoundedDate,
                leagueId = 999999
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameMissing()
        {
            var league = await SeedLeagueAsync("Bundesliga");

            var response = await _adminClient.PostAsJsonAsync("/api/clubs", new
            {
                name = "",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameAlreadyExists()
        {
            var league = await SeedLeagueAsync("Ligue 1");
            await SeedClubAsync(league.Id, "Existing Club");

            var response = await _adminClient.PostAsJsonAsync("/api/clubs", new
            {
                name = "Existing Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await _factory.WithDbContextAsync(async db =>
            {
                var count = await db.Clubs.CountAsync(c => c.Name == "Existing Club");
                count.Should().Be(1);
            });
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesClub_AndReturns200()
        {
            var league = await SeedLeagueAsync("Eredivisie");
            var club   = await SeedClubAsync(league.Id, "Old Club");

            var response = await _adminClient.PutAsJsonAsync($"/api/clubs/{club.Id}", new
            {
                name = "New Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ClubDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(club.Id);
            dto.Name.Should().Be("New Club");

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Clubs.FindAsync(club.Id);
                updated!.Name.Should().Be("New Club");
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenClubMissing()
        {
            var league = await SeedLeagueAsync("Primeira Liga");

            var response = await _adminClient.PutAsJsonAsync("/api/clubs/999999", new
            {
                name = "Whatever",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesClub_AndReturns204()
        {
            var league = await SeedLeagueAsync("Scottish Premiership");
            var club   = await SeedClubAsync(league.Id, "To Delete");

            var response = await _adminClient.DeleteAsync($"/api/clubs/{club.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Clubs.FindAsync(club.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenClubMissing()
        {
            var response = await _adminClient.DeleteAsync("/api/clubs/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── AUTORIZACIJA ───────────────────────────

        [Fact]
        public async Task Post_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PostAsJsonAsync("/api/clubs", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Post_Returns403_WhenRegularUser()
        {
            // Kreiranje kluba je samo za admina (kao na webu).
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PostAsJsonAsync("/api/clubs", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Put_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PutAsJsonAsync("/api/clubs/1", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PutAsJsonAsync("/api/clubs/1", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.DeleteAsync("/api/clubs/1");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.DeleteAsync("/api/clubs/1");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
