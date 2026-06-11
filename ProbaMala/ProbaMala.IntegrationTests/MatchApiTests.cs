using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za MatchApiController — najsloženiji entitet:
    // dva FK na Club (home/away) + FK na League, i ValidateTeams s pravilima (liga
    // postoji, home ≠ away, oba kluba moraju biti u toj ligi). Seed helper zato kreira
    // ligu s dva kluba. Svaki test dobije svoj factory → svježu, izoliranu bazu.
    public class MatchApiTests : IDisposable
    {
        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;        // neautentificiran
        private readonly HttpClient _adminClient;   // admin rola

        public MatchApiTests()
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

        private async Task<(League league, Club home, Club away)> SeedLeagueWithTwoClubsAsync(
            string leagueName, string homeName, string awayName)
        {
            var league = new League { Name = leagueName };
            var home   = new Club { Name = homeName, FoundedDate = new DateTime(1900, 1, 1), League = league };
            var away   = new Club { Name = awayName, FoundedDate = new DateTime(1900, 1, 1), League = league };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Clubs.AddRange(home, away);
                await db.SaveChangesAsync();
            });
            return (league, home, away);
        }

        private async Task<Match> SeedMatchAsync(
            int leagueId, int homeId, int awayId, DateTime date, int homeGoals = 1, int awayGoals = 0)
        {
            var match = new Match
            {
                LeagueId   = leagueId,
                HomeTeamId = homeId,
                AwayTeamId = awayId,
                Date       = date,
                HomeGoals  = homeGoals,
                AwayGoals  = awayGoals
            };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Matches.Add(match);
                await db.SaveChangesAsync();
            });
            return match;
        }

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllMatches_OrderedByDateDescending()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("Premier League", "Home FC", "Away FC");
            await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 1, 1));
            await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 3, 1));
            await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 2, 1));

            var response = await _client.GetAsync("/api/matches");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var matches = await response.Content.ReadFromJsonAsync<List<MatchDTO>>();
            matches.Should().NotBeNull();
            matches!.Select(m => m.Date).Should().Equal(
                new DateTime(2024, 3, 1), new DateTime(2024, 2, 1), new DateTime(2024, 1, 1));
            matches.Should().OnlyContain(m =>
                m.League.Name == "Premier League" && m.HomeTeam.Name == "Home FC" && m.AwayTeam.Name == "Away FC");
        }

        [Fact]
        public async Task GetAll_FiltersByLeagueId_WhenProvided()
        {
            var a = await SeedLeagueWithTwoClubsAsync("League A", "A Home", "A Away");
            var b = await SeedLeagueWithTwoClubsAsync("League B", "B Home", "B Away");
            await SeedMatchAsync(a.league.Id, a.home.Id, a.away.Id, new DateTime(2024, 1, 1));
            await SeedMatchAsync(b.league.Id, b.home.Id, b.away.Id, new DateTime(2024, 1, 1));

            var response = await _client.GetAsync($"/api/matches?leagueId={a.league.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var matches = await response.Content.ReadFromJsonAsync<List<MatchDTO>>();
            matches.Should().ContainSingle();
            matches![0].League.Id.Should().Be(a.league.Id);
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsMatch_WhenMatchExists()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("La Liga", "Real", "Barca");
            var match = await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 5, 1), 2, 3);

            var response = await _client.GetAsync($"/api/matches/{match.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<MatchDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(match.Id);
            dto.Date.Should().Be(new DateTime(2024, 5, 1));
            dto.HomeGoals.Should().Be(2);
            dto.AwayGoals.Should().Be(3);
            dto.League.Id.Should().Be(league.Id);
            dto.HomeTeam.Id.Should().Be(home.Id);
            dto.AwayTeam.Id.Should().Be(away.Id);
        }

        [Fact]
        public async Task GetById_Returns404_WhenMatchMissing()
        {
            var response = await _client.GetAsync("/api/matches/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesMatch_AndReturns201()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("Serie A", "Inter", "Milan");

            var response = await _adminClient.PostAsJsonAsync("/api/matches", new
            {
                date       = new DateTime(2024, 4, 10),
                homeGoals  = 1,
                awayGoals  = 1,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<MatchDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.HomeTeam.Id.Should().Be(home.Id);
            dto.AwayTeam.Id.Should().Be(away.Id);

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Matches.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.LeagueId.Should().Be(league.Id);
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenLeagueDoesNotExist()
        {
            var (_, home, away) = await SeedLeagueWithTwoClubsAsync("Bundesliga", "Bayern", "Dortmund");

            var response = await _adminClient.PostAsJsonAsync("/api/matches", new
            {
                date       = new DateTime(2024, 4, 10),
                homeGoals  = 0,
                awayGoals  = 0,
                leagueId   = 999999,
                homeTeamId = home.Id,
                awayTeamId = away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenHomeAndAwayAreSame()
        {
            var (league, home, _) = await SeedLeagueWithTwoClubsAsync("Ligue 1", "PSG", "Marseille");

            var response = await _adminClient.PostAsJsonAsync("/api/matches", new
            {
                date       = new DateTime(2024, 4, 10),
                homeGoals  = 0,
                awayGoals  = 0,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = home.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenTeamNotInSpecifiedLeague()
        {
            var a = await SeedLeagueWithTwoClubsAsync("League A", "A Home", "A Away");
            var b = await SeedLeagueWithTwoClubsAsync("League B", "B Home", "B Away");

            var response = await _adminClient.PostAsJsonAsync("/api/matches", new
            {
                date       = new DateTime(2024, 4, 10),
                homeGoals  = 0,
                awayGoals  = 0,
                leagueId   = b.league.Id,
                homeTeamId = a.home.Id,
                awayTeamId = b.away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenGoalsOutOfRange()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("Eredivisie", "Ajax", "PSV");

            var response = await _adminClient.PostAsJsonAsync("/api/matches", new
            {
                date       = new DateTime(2024, 4, 10),
                homeGoals  = 100,
                awayGoals  = 0,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesMatch_AndReturns200()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("Primeira Liga", "Porto", "Benfica");
            var match = await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 1, 1), 0, 0);

            var response = await _adminClient.PutAsJsonAsync($"/api/matches/{match.Id}", new
            {
                date       = new DateTime(2024, 1, 1),
                homeGoals  = 3,
                awayGoals  = 2,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<MatchDTO>();
            dto.Should().NotBeNull();
            dto!.HomeGoals.Should().Be(3);
            dto.AwayGoals.Should().Be(2);

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Matches.FindAsync(match.Id);
                updated!.HomeGoals.Should().Be(3);
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenMatchMissing()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("MLS", "LA", "NY");

            var response = await _adminClient.PutAsJsonAsync("/api/matches/999999", new
            {
                date       = new DateTime(2024, 1, 1),
                homeGoals  = 0,
                awayGoals  = 0,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = away.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Returns400_WhenHomeAndAwayAreSame()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("J1 League", "Tokyo", "Osaka");
            var match = await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 1, 1));

            var response = await _adminClient.PutAsJsonAsync($"/api/matches/{match.Id}", new
            {
                date       = new DateTime(2024, 1, 1),
                homeGoals  = 0,
                awayGoals  = 0,
                leagueId   = league.Id,
                homeTeamId = home.Id,
                awayTeamId = home.Id
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesMatch_AndReturns204()
        {
            var (league, home, away) = await SeedLeagueWithTwoClubsAsync("Liga MX", "Tigres", "Rayados");
            var match = await SeedMatchAsync(league.Id, home.Id, away.Id, new DateTime(2024, 1, 1));

            var response = await _adminClient.DeleteAsync($"/api/matches/{match.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Matches.FindAsync(match.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenMatchMissing()
        {
            var response = await _adminClient.DeleteAsync("/api/matches/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── AUTORIZACIJA ───────────────────────────

        [Fact]
        public async Task Post_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PostAsJsonAsync("/api/matches", new { });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PutAsJsonAsync("/api/matches/1", new { });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PutAsJsonAsync("/api/matches/1", new { });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.DeleteAsync("/api/matches/1");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Returns403_WhenRegularUser()
        {
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.DeleteAsync("/api/matches/1");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
