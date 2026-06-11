using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ProbaMala.Models.DTOs;
using ProbaMala.Models.Entities;

namespace ProbaMala.IntegrationTests
{
    // End-to-end integracijski testovi za RatingApiController — najpovezaniji entitet:
    // FK na Player, Match i (domenski) User. Seed helper zato kreira cijeli graf
    // Liga → 2 kluba → Player → Match → User. ValidateRefs traži da sva tri reference
    // postoje. Svaki test dobije svoj factory → svježu, izoliranu bazu.
    //
    // Auth pravila koja testovi pokrivaju:
    //   POST   → [Authorize]          (svaki prijavljeni korisnik)
    //   PUT    → [Authorize(Admin)]   (samo admin)
    //   DELETE → [Authorize] + vlasnik-ili-admin (korisnik briše vlastitu; admin briše sve)
    public class RatingApiTests : IDisposable
    {
        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;        // neautentificiran
        private readonly HttpClient _adminClient;   // admin rola

        public RatingApiTests()
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

        // Seedaj cijeli graf ovisnosti za rating i vrati tri reference koje request treba.
        // userAppUserId — postavlja User.AppUserId kako bi vlasništvo bilo testirano.
        private async Task<(Player player, Match match, User user)> SeedRatingDependenciesAsync(
            string? userAppUserId = null)
        {
            var league = new League { Name = "Premier League" };
            var home   = new Club { Name = "Home FC", FoundedDate = new DateTime(1900, 1, 1), League = league };
            var away   = new Club { Name = "Away FC", FoundedDate = new DateTime(1900, 1, 1), League = league };
            var player = new Player
            {
                FirstName   = "Test",
                LastName    = "Player",
                DateOfBirth = new DateTime(2000, 1, 1),
                Position    = Position.Forward,
                Nationality = "Croatia",
                Club        = home
            };
            var match = new Match
            {
                League    = league,
                HomeTeam  = home,
                AwayTeam  = away,
                Date      = new DateTime(2024, 1, 1),
                HomeGoals = 1,
                AwayGoals = 0
            };
            var user = new User
            {
                FirstName  = "John",
                LastName   = "Doe",
                Email      = "john@example.com",
                AppUserId  = userAppUserId
            };

            await _factory.WithDbContextAsync(async db =>
            {
                db.Players.Add(player);
                db.Matches.Add(match);
                db.Users.Add(user);
                await db.SaveChangesAsync();
            });

            return (player, match, user);
        }

        private async Task<Rating> SeedRatingAsync(
            int playerId, int matchId, int userId, int score, string? comment = null)
        {
            var rating = new Rating
            {
                PlayerId = playerId,
                MatchId  = matchId,
                UserId   = userId,
                Score    = score,
                Comment  = comment
            };
            await _factory.WithDbContextAsync(async db =>
            {
                db.Ratings.Add(rating);
                await db.SaveChangesAsync();
            });
            return rating;
        }

        // ─────────────────────────── GET all ───────────────────────────

        [Fact]
        public async Task GetAll_ReturnsAllRatings_OrderedByScoreDescending()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            await SeedRatingAsync(player.Id, match.Id, user.Id, 5);
            await SeedRatingAsync(player.Id, match.Id, user.Id, 9);
            await SeedRatingAsync(player.Id, match.Id, user.Id, 7);

            var response = await _client.GetAsync("/api/ratings");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ratings = await response.Content.ReadFromJsonAsync<List<RatingDTO>>();
            ratings.Should().NotBeNull();
            ratings!.Select(r => r.Score).Should().Equal(9, 7, 5);
            ratings.Should().OnlyContain(r =>
                r.Player.FullName == "Test Player" && r.User.FullName == "John Doe");
        }

        [Fact]
        public async Task GetAll_FiltersByQuery_WhenQProvided()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            await SeedRatingAsync(player.Id, match.Id, user.Id, 5, "amazing");
            await SeedRatingAsync(player.Id, match.Id, user.Id, 6, "poor");

            var response = await _client.GetAsync("/api/ratings?q=amazing");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ratings = await response.Content.ReadFromJsonAsync<List<RatingDTO>>();
            ratings.Should().ContainSingle();
            ratings![0].Comment.Should().Be("amazing");
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsRating_WhenRatingExists()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 8, "Great game");

            var response = await _client.GetAsync($"/api/ratings/{rating.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<RatingDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(rating.Id);
            dto.Score.Should().Be(8);
            dto.Comment.Should().Be("Great game");
            dto.Player.Id.Should().Be(player.Id);
            dto.Player.FullName.Should().Be("Test Player");
            dto.Player.Position.Should().Be("Forward");
            dto.Match.Id.Should().Be(match.Id);
            dto.Match.HomeTeamName.Should().Be("Home FC");
            dto.Match.AwayTeamName.Should().Be("Away FC");
            dto.User.Id.Should().Be(user.Id);
            dto.User.FullName.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetById_Returns404_WhenRatingMissing()
        {
            var response = await _client.GetAsync("/api/ratings/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── POST ───────────────────────────

        [Fact]
        public async Task Post_CreatesRating_AndReturns201()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = user.Id,
                score    = 9,
                comment  = "Man of the match"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var dto = await response.Content.ReadFromJsonAsync<RatingDTO>();
            dto.Should().NotBeNull();
            dto!.Id.Should().BeGreaterThan(0);
            dto.Score.Should().Be(9);
            dto.Player.Id.Should().Be(player.Id);

            await _factory.WithDbContextAsync(async db =>
            {
                var exists = await db.Ratings.FindAsync(dto.Id);
                exists.Should().NotBeNull();
                exists!.Score.Should().Be(9);
            });
        }

        [Fact]
        public async Task Post_Returns400_WhenPlayerDoesNotExist()
        {
            var (_, match, user) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PostAsJsonAsync("/api/ratings", new
            {
                playerId = 999999,
                matchId  = match.Id,
                userId   = user.Id,
                score    = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenMatchDoesNotExist()
        {
            var (player, _, user) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId  = 999999,
                userId   = user.Id,
                score    = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenUserDoesNotExist()
        {
            var (player, match, _) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = 999999,
                score    = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenScoreOutOfRange()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = user.Id,
                score    = 11
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesRating_AndReturns200()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 4, "meh");

            var response = await _adminClient.PutAsJsonAsync($"/api/ratings/{rating.Id}", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = user.Id,
                score    = 10,
                comment  = "Brilliant"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<RatingDTO>();
            dto.Should().NotBeNull();
            dto!.Score.Should().Be(10);
            dto.Comment.Should().Be("Brilliant");

            await _factory.WithDbContextAsync(async db =>
            {
                var updated = await db.Ratings.FindAsync(rating.Id);
                updated!.Score.Should().Be(10);
            });
        }

        [Fact]
        public async Task Put_Returns404_WhenRatingMissing()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();

            var response = await _adminClient.PutAsJsonAsync("/api/ratings/999999", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = user.Id,
                score    = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Returns400_WhenUserDoesNotExist()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 5);

            var response = await _adminClient.PutAsJsonAsync($"/api/ratings/{rating.Id}", new
            {
                playerId = player.Id,
                matchId  = match.Id,
                userId   = 999999,
                score    = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesRating_AndReturns204()
        {
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 6);

            var response = await _adminClient.DeleteAsync($"/api/ratings/{rating.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Ratings.FindAsync(rating.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns404_WhenRatingMissing()
        {
            var response = await _adminClient.DeleteAsync("/api/ratings/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────── AUTORIZACIJA ───────────────────────────

        [Fact]
        public async Task Post_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PostAsJsonAsync("/api/ratings", new { score = 5 });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.PutAsJsonAsync("/api/ratings/1", new { score = 5 });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Put_Returns403_WhenRegularUser()
        {
            // PUT je samo za admina; obični korisnik dobiva 403 Forbidden.
            using var userClient = _factory.CreateUserClient("some-user-id");
            var response = await userClient.PutAsJsonAsync("/api/ratings/1", new { score = 5 });
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_Returns401_WhenNotAuthenticated()
        {
            var response = await _client.DeleteAsync("/api/ratings/1");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_Returns204_WhenOwner()
        {
            // Korisnik briše vlastitu ocjenu (User.AppUserId odgovara prijavljenom korisniku).
            const string ownerAppId = "owner-app-id";
            var (player, match, user) = await SeedRatingDependenciesAsync(ownerAppId);
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 7);

            using var ownerClient = _factory.CreateUserClient(ownerAppId);
            var response = await ownerClient.DeleteAsync($"/api/ratings/{rating.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await _factory.WithDbContextAsync(async db =>
            {
                var gone = await db.Ratings.FindAsync(rating.Id);
                gone.Should().BeNull();
            });
        }

        [Fact]
        public async Task Delete_Returns403_WhenNonOwner()
        {
            // Tuđa ocjena → 403 Forbidden; vlasništvo se provjerava po AppUserId.
            const string ownerAppId = "owner-app-id";
            const string otherAppId = "other-app-id";
            var (player, match, user) = await SeedRatingDependenciesAsync(ownerAppId);
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 7);

            using var otherClient = _factory.CreateUserClient(otherAppId);
            var response = await otherClient.DeleteAsync($"/api/ratings/{rating.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
