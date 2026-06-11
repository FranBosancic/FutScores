using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za LeagueApiController kroz stvarni HTTP sloj —
    // ruta, model binding, validacija, EF query i JSON DTO. Svaki test radi svoj
    // factory (u konstruktoru), pa svaki dobije svježu, izoliranu in-memory bazu.
    public class LeagueApiTests : IDisposable
    {
        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;        // neautentificiran
        private readonly HttpClient _adminClient;   // admin rola

        public LeagueApiTests()
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

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllLeagues_OrderedByName()
        {
            await SeedLeagueAsync("Zeta League");
            await SeedLeagueAsync("Alpha League");
            await SeedLeagueAsync("Mid League");

            var response = await _client.GetAsync("/api/leagues");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var leagues = await response.Content.ReadFromJsonAsync<List<LeagueDTO>>();
            leagues.Should().NotBeNull();
            leagues!.Select(l => l.Name)
                .Should().Equal("Alpha League", "Mid League", "Zeta League");
        }

        [Fact]
        public async Task GetAll_FiltersByQuery_WhenQProvided()
        {
            await SeedLeagueAsync("La Liga");
            await SeedLeagueAsync("Premier League");

            var response = await _client.GetAsync("/api/leagues?q=liga");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var leagues = await response.Content.ReadFromJsonAsync<List<LeagueDTO>>();
            leagues.Should().ContainSingle()
                .Which.Name.Should().Be("La Liga");
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsLeague_WhenLeagueExists()
        {
            var seeded = await SeedLeagueAsync("Test League");

            var response = await _client.GetAsync($"/api/leagues/{seeded.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<LeagueDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(seeded.Id);
            dto.Name.Should().Be("Test League");
            dto.ClubCount.Should().Be(0);
            dto.MatchCount.Should().Be(0);
        }

        [Fact]
        public async Task GetById_Returns404_WhenLeagueMissing()
        {
            var response = await _client.GetAsync("/api/leagues/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesLeague_AndReturns201()
        {
            var response = await _adminClient.PostAsJsonAsync("/api/leagues", new { name = "New League" });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<LeagueDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.Name.Should().Be("New League");

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Leagues.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.Name.Should().Be("New League");
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenNameMissing()
        {
            var response = await _adminClient.PostAsJsonAsync("/api/leagues", new { name = "" });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameAlreadyExists()
        {
            await SeedLeagueAsync("Existing League");

            var response = await _adminClient.PostAsJsonAsync("/api/leagues", new { name = "Existing League" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            await _factory.WithDbContextAsync(async db =>
            {
                var count = await db.Leagues.CountAsync(l => l.Name == "Existing League");
                count.Should().Be(1);
            });
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesLeague_AndReturns200()
        {
            var seeded = await SeedLeagueAsync("Old Name");

            var response = await _adminClient.PutAsJsonAsync($"/api/leagues/{seeded.Id}", new { name = "New Name" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<LeagueDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(seeded.Id);
            dto.Name.Should().Be("New Name");

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Leagues.FindAsync(seeded.Id);
                updated!.Name.Should().Be("New Name");
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenLeagueMissing()
        {
            var response = await _adminClient.PutAsJsonAsync("/api/leagues/999999", new { name = "Whatever" });
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesLeague_AndReturns204()
        {
            var seeded = await SeedLeagueAsync("To Delete");

            var response = await _adminClient.DeleteAsync($"/api/leagues/{seeded.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Leagues.FindAsync(seeded.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenLeagueMissing()
        {
            var response = await _adminClient.DeleteAsync("/api/leagues/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── AUTORIZACIJA ───────────────────────────

        [Fact]
        public async Task Post_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PostAsJsonAsync("/api/leagues", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PutAsJsonAsync("/api/leagues/1", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PutAsJsonAsync("/api/leagues/1", new { name = "X" });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.DeleteAsync("/api/leagues/1");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.DeleteAsync("/api/leagues/1");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
