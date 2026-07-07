using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class RatingsApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public RatingsApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/ratings");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        // A rating needs a player, a match and an author (domain user).
        var (leagueId, homeId, awayId, matchId) = await _fixture.CreateMatchAsync();
        int playerId = 0, userId = 0, ratingId = 0;
        try
        {
            var player = await Api.PostAsync("/api/players", new()
            {
                DataObject = new { firstName = "PW", lastName = $"P{ApiFixture.Tag()}", dateOfBirth = "1999-01-01", position = 2, nationality = "Testland", clubId = homeId }
            });
            Assert.Equal(201, player.Status);
            playerId = await player.IdAsync();

            userId = await _fixture.CreateUserAsync();

            var create = await Api.PostAsync("/api/ratings", new() { DataObject = new { playerId, matchId, userId, score = 7, comment = "solid" } });
            Assert.Equal(201, create.Status);
            ratingId = await create.IdAsync();

            Assert.Equal(200, (await Api.GetAsync($"/api/ratings/{ratingId}")).Status);

            var put = await Api.PutAsync($"/api/ratings/{ratingId}", new() { DataObject = new { playerId, matchId, userId, score = 9, comment = "even better" } });
            Assert.Equal(200, put.Status);
            Assert.Equal(9, (await put.JsonElementAsync()).GetProperty("score").GetInt32());

            Assert.Equal(204, (await Api.DeleteAsync($"/api/ratings/{ratingId}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/ratings/{ratingId}")).Status);
            ratingId = 0;
        }
        finally
        {
            if (ratingId != 0) await _fixture.TryDeleteAsync($"/api/ratings/{ratingId}");
            if (playerId != 0) await _fixture.TryDeleteAsync($"/api/players/{playerId}");
            if (userId != 0)   await _fixture.TryDeleteAsync($"/api/users/{userId}");
            await _fixture.TryDeleteAsync($"/api/matches/{matchId}");
            await _fixture.TryDeleteAsync($"/api/clubs/{homeId}");
            await _fixture.TryDeleteAsync($"/api/clubs/{awayId}");
            await _fixture.TryDeleteAsync($"/api/leagues/{leagueId}");
        }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/ratings/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/ratings/{missing}", new() { DataObject = new { playerId = 1, matchId = 1, userId = 1, score = 5, comment = "x" } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/ratings/{missing}")).Status);
    }

    [Fact]
    public async Task Post_UnknownPlayer_Returns400()
    {
        var resp = await Api.PostAsync("/api/ratings", new() { DataObject = new { playerId = 999999, matchId = 1, userId = 1, score = 5, comment = "x" } });
        Assert.Equal(400, resp.Status);
    }
}
