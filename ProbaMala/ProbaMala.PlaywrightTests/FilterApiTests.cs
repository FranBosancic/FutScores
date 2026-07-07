using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Smoke-tests the query-string filter variants of the list endpoints (the same routes,
// but exercising their optional filters). Seed data: league id 1 = Premier League,
// clubs 1/2 = Arsenal/Manchester City, position 3 = Forward.
[Collection("api")]
public class FilterApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public FilterApiTests(ApiFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("/api/leagues?q=premier")]
    [InlineData("/api/clubs?leagueId=1")]
    [InlineData("/api/clubs?q=arsenal")]
    [InlineData("/api/players?q=salah")]
    [InlineData("/api/players?clubId=1")]
    [InlineData("/api/players?leagueId=1")]
    [InlineData("/api/players?position=3")]
    [InlineData("/api/matches?leagueId=1")]
    [InlineData("/api/matches?clubId=1")]
    [InlineData("/api/ratings?q=arsenal")]
    [InlineData("/api/ratings?minScore=7&maxScore=10")]
    [InlineData("/api/users?q=a")]
    public async Task FilteredList_ReturnsOkArray(string url)
    {
        var resp = await Api.GetAsync(url);
        Assert.True(resp.Status == 200, $"{url} -> {resp.Status}");
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }
}
