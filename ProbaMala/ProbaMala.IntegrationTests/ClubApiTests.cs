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
        private readonly HttpClient _client;

        public ClubApiTests()
        {
            _factory = new FutScoresApiFactory();
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // Pomoćne metode: liga je FK pa je klubu uvijek treba prvo.
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
            // Arrange
            var league = await SeedLeagueAsync("Premier League");
            await SeedClubAsync(league.Id, "Zeta FC");
            await SeedClubAsync(league.Id, "Alpha FC");
            await SeedClubAsync(league.Id, "Mid FC");

            // Act
            var response = await _client.GetAsync("/api/clubs");

            // Assert — 200, poredano po imenu, i ugniježđena liga je popunjena.
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var clubs = await response.Content.ReadFromJsonAsync<List<ClubDTO>>();
            clubs.Should().NotBeNull();
            clubs!.Select(c => c.Name).Should().Equal("Alpha FC", "Mid FC", "Zeta FC");
            clubs.Should().OnlyContain(c => c.League.Name == "Premier League");
        }

        [Fact]
        public async Task GetAll_FiltersByLeagueId_WhenProvided()
        {
            // Arrange — dvije lige, po jedan klub u svakoj.
            var first = await SeedLeagueAsync("First League");
            var second = await SeedLeagueAsync("Second League");
            await SeedClubAsync(first.Id, "First Club");
            await SeedClubAsync(second.Id, "Second Club");

            // Act — filtriraj samo po prvoj ligi.
            var response = await _client.GetAsync($"/api/clubs?leagueId={first.Id}");

            // Assert
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
            // Arrange
            var league = await SeedLeagueAsync("La Liga");
            var club = await SeedClubAsync(league.Id, "Test Club");

            // Act
            var response = await _client.GetAsync($"/api/clubs/{club.Id}");

            // Assert — DTO + ugniježđeni LeagueSummaryDTO + izračunati brojači.
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
            // Arrange — liga mora postojati (FK).
            var league = await SeedLeagueAsync("Serie A");

            // Act
            var response = await _client.PostAsJsonAsync("/api/clubs", new
            {
                name = "New Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            // Assert — 201 + Location + DTO s ugniježđenom ligom, i zapis u bazi.
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
            // Act — valjan model, ali nepostojeća liga → business pravilo 400.
            var response = await _client.PostAsJsonAsync("/api/clubs", new
            {
                name = "Orphan Club",
                foundedDate = SampleFoundedDate,
                leagueId = 999999
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameMissing()
        {
            // Arrange
            var league = await SeedLeagueAsync("Bundesliga");

            // Act — prazno ime pada na [Required] validaciji.
            var response = await _client.PostAsJsonAsync("/api/clubs", new
            {
                name = "",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenNameAlreadyExists()
        {
            // Arrange — klub s tim imenom već postoji.
            var league = await SeedLeagueAsync("Ligue 1");
            await SeedClubAsync(league.Id, "Existing Club");

            // Act
            var response = await _client.PostAsJsonAsync("/api/clubs", new
            {
                name = "Existing Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            // Assert — 400 i nije nastao drugi zapis.
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
            // Arrange
            var league = await SeedLeagueAsync("Eredivisie");
            var club = await SeedClubAsync(league.Id, "Old Club");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/clubs/{club.Id}", new
            {
                name = "New Club",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            // Assert — 200 + DTO s novim imenom, i promjena je u bazi.
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
            // Arrange — valjana liga da model prođe validaciju; klub ne postoji.
            var league = await SeedLeagueAsync("Primeira Liga");

            // Act
            var response = await _client.PutAsJsonAsync("/api/clubs/999999", new
            {
                name = "Whatever",
                foundedDate = SampleFoundedDate,
                leagueId = league.Id
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesClub_AndReturns204()
        {
            // Arrange — klub bez igrača/utakmica (smije se obrisati).
            var league = await SeedLeagueAsync("Scottish Premiership");
            var club = await SeedClubAsync(league.Id, "To Delete");

            // Act
            var response = await _client.DeleteAsync($"/api/clubs/{club.Id}");

            // Assert — 204 i zapisa više nema.
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
            var response = await _client.DeleteAsync("/api/clubs/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
