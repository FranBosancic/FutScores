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
    public class RatingApiTests : IDisposable
    {
        private readonly FutScoresApiFactory _factory;
        private readonly HttpClient _client;

        public RatingApiTests()
        {
            _factory = new FutScoresApiFactory();
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        // Seedaj cijeli graf ovisnosti za rating i vrati tri reference koje request treba.
        private async Task<(Player player, Match match, User user)> SeedRatingDependenciesAsync()
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
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            await _factory.WithDbContextAsync(async db =>
            {
                // EF otkriva ligu/klubove kroz navigacije, pa je dovoljno dodati korijene.
                db.Players.Add(player);
                db.Matches.Add(match);
                db.Users.Add(user);
                await db.SaveChangesAsync();
            });

            return (player, match, user);
        }

        private async Task<Rating> SeedRatingAsync(int playerId, int matchId, int userId, int score, string? comment = null)
        {
            var rating = new Rating
            {
                PlayerId = playerId,
                MatchId = matchId,
                UserId = userId,
                Score = score,
                Comment = comment
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
            // Arrange — tri ocjene istog igrača/utakmice/korisnika, različiti score.
            var (player, match, user) = await SeedRatingDependenciesAsync();
            await SeedRatingAsync(player.Id, match.Id, user.Id, 5);
            await SeedRatingAsync(player.Id, match.Id, user.Id, 9);
            await SeedRatingAsync(player.Id, match.Id, user.Id, 7);

            // Act
            var response = await _client.GetAsync("/api/ratings");

            // Assert — 200 + poredak po score silazno, ugniježđeni objekti popunjeni.
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
            // Arrange — dvije ocjene s različitim komentarima.
            var (player, match, user) = await SeedRatingDependenciesAsync();
            await SeedRatingAsync(player.Id, match.Id, user.Id, 5, "amazing");
            await SeedRatingAsync(player.Id, match.Id, user.Id, 6, "poor");

            // Act — pretraga po komentaru.
            var response = await _client.GetAsync("/api/ratings?q=amazing");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ratings = await response.Content.ReadFromJsonAsync<List<RatingDTO>>();
            ratings.Should().ContainSingle();
            ratings![0].Comment.Should().Be("amazing");
        }

        // ─────────────────────────── GET by id ───────────────────────────

        [Fact]
        public async Task GetById_ReturnsRating_WhenRatingExists()
        {
            // Arrange
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 8, "Great game");

            // Act
            var response = await _client.GetAsync($"/api/ratings/{rating.Id}");

            // Assert — DTO s tri ugniježđena summarya (player/match/user).
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
            // Arrange
            var (player, match, user) = await SeedRatingDependenciesAsync();

            // Act
            var response = await _client.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = user.Id,
                score = 9,
                comment = "Man of the match"
            });

            // Assert — 201 + Location + DTO, i zapis u bazi.
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

            var response = await _client.PostAsJsonAsync("/api/ratings", new
            {
                playerId = 999999,
                matchId = match.Id,
                userId = user.Id,
                score = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenMatchDoesNotExist()
        {
            var (player, _, user) = await SeedRatingDependenciesAsync();

            var response = await _client.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId = 999999,
                userId = user.Id,
                score = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenUserDoesNotExist()
        {
            var (player, match, _) = await SeedRatingDependenciesAsync();

            var response = await _client.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = 999999,
                score = 5
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Returns400_WhenScoreOutOfRange()
        {
            // Arrange — sve reference valjane, ali score izvan [Range(1,10)].
            var (player, match, user) = await SeedRatingDependenciesAsync();

            // Act
            var response = await _client.PostAsJsonAsync("/api/ratings", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = user.Id,
                score = 11
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── PUT ───────────────────────────

        [Fact]
        public async Task Put_UpdatesRating_AndReturns200()
        {
            // Arrange
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 4, "meh");

            // Act
            var response = await _client.PutAsJsonAsync($"/api/ratings/{rating.Id}", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = user.Id,
                score = 10,
                comment = "Brilliant"
            });

            // Assert — 200 + DTO s novim vrijednostima, i promjena u bazi.
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
            // Arrange — valjane reference, ali rating ne postoji.
            var (player, match, user) = await SeedRatingDependenciesAsync();

            // Act
            var response = await _client.PutAsJsonAsync("/api/ratings/999999", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = user.Id,
                score = 5
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Returns400_WhenUserDoesNotExist()
        {
            // Arrange — rating postoji, ali ga vežemo na nepostojećeg korisnika.
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 5);

            // Act
            var response = await _client.PutAsJsonAsync($"/api/ratings/{rating.Id}", new
            {
                playerId = player.Id,
                matchId = match.Id,
                userId = 999999,
                score = 5
            });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────── DELETE ───────────────────────────

        [Fact]
        public async Task Delete_RemovesRating_AndReturns204()
        {
            // Arrange
            var (player, match, user) = await SeedRatingDependenciesAsync();
            var rating = await SeedRatingAsync(player.Id, match.Id, user.Id, 6);

            // Act
            var response = await _client.DeleteAsync($"/api/ratings/{rating.Id}");

            // Assert — 204 i zapisa više nema.
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
            var response = await _client.DeleteAsync("/api/ratings/999999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
