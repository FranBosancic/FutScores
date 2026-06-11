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
        private readonly HttpClient _client;

        public LeagueApiTests()
        {
            // xUnit radi novu instancu razreda za svaki test, pa novi factory ovdje
            // znači svježu, izoliranu in-memory bazu po svakom testu.
            _factory = new FutScoresApiFactory();
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // Pomoćna metoda: seedaj jednu ligu izravno u bazu i vrati je (s dodijeljenim Id-em).
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
            // Arrange — seedaj tri lige nesortirano.
            await SeedLeagueAsync("Zeta League");
            await SeedLeagueAsync("Alpha League");
            await SeedLeagueAsync("Mid League");

            // Act
            var response = await _client.GetAsync("/api/leagues");

            // Assert — 200 + sve tri lige, poredane po imenu (kako controller i radi).
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var leagues = await response.Content.ReadFromJsonAsync<List<LeagueDTO>>();
            leagues.Should().NotBeNull();
            leagues!.Select(l => l.Name)
                .Should().Equal("Alpha League", "Mid League", "Zeta League");
        }

        [Fact]
        public async Task GetAll_FiltersByQuery_WhenQProvided()
        {
            // Arrange
            await SeedLeagueAsync("La Liga");
            await SeedLeagueAsync("Premier League");

            // Act — pretraga "liga" odgovara samo "La Liga".
            var response = await _client.GetAsync("/api/leagues?q=liga");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var leagues = await response.Content.ReadFromJsonAsync<List<LeagueDTO>>();
            leagues.Should().ContainSingle()
                .Which.Name.Should().Be("La Liga");
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsLeague_WhenLeagueExists()
        {
            // Arrange — seedaj točno jednu ligu izravno u in-memory bazu.
            var seeded = await SeedLeagueAsync("Test League");

            // Act — pozovi stvarni endpoint preko HTTP-a.
            var response = await _client.GetAsync($"/api/leagues/{seeded.Id}");

            // Assert — HTTP status i JSON DTO koji klijent stvarno dobije.
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
            // Act — nepostojeći Id u praznoj bazi.
            var response = await _client.GetAsync("/api/leagues/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesLeague_AndReturns201()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/leagues", new { name = "New League" });

            // Assert — 201 Created + Location na novi resurs + DTO u tijelu.
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<LeagueDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.Name.Should().Be("New League");

            // I doista je spremljena u bazu.
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
            // Act — prazno ime pada na [Required] validaciji ([ApiController] → 400).
            var response = await _client.PostAsJsonAsync("/api/leagues", new { name = "" });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameAlreadyExists()
        {
            // Arrange — liga s tim imenom već postoji.
            await SeedLeagueAsync("Existing League");

            // Act — pokušaj kreirati duplikat (business pravilo u controlleru → 400).
            var response = await _client.PostAsJsonAsync("/api/leagues", new { name = "Existing League" });

            // Assert — 400 i nije nastao drugi zapis.
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
            // Arrange
            var seeded = await SeedLeagueAsync("Old Name");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/leagues/{seeded.Id}", new { name = "New Name" });

            // Assert — 200 + DTO s novim imenom, i promjena je u bazi.
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
            // Act — valjan model, ali nepostojeći Id → controller vraća NotFound.
            var response = await _client.PutAsJsonAsync("/api/leagues/999999", new { name = "Whatever" });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesLeague_AndReturns204()
        {
            // Arrange — liga bez klubova/utakmica (smije se obrisati).
            var seeded = await SeedLeagueAsync("To Delete");

            // Act
            var response = await _client.DeleteAsync($"/api/leagues/{seeded.Id}");

            // Assert — 204 No Content i zapisa više nema u bazi.
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
            // Act
            var response = await _client.DeleteAsync("/api/leagues/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
