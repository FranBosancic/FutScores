using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class MatchesApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public MatchesApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/matches");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        // CreateMatchAsync seeds the league + two clubs + the match (exercises POST /api/matches).
        var (leagueId, homeId, awayId, matchId) = await _fixture.CreateMatchAsync();
        try
        {
            Assert.Equal(200, (await Api.GetAsync($"/api/matches/{matchId}")).Status);

            var put = await Api.PutAsync($"/api/matches/{matchId}", new()
            {
                DataObject = new { leagueId, homeTeamId = homeId, awayTeamId = awayId, date = "2026-06-01T20:00:00", homeGoals = 3, awayGoals = 3 }
            });
            Assert.Equal(200, put.Status);

            Assert.Equal(204, (await Api.DeleteAsync($"/api/matches/{matchId}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/matches/{matchId}")).Status);
            matchId = 0;
        }
        finally
        {
            if (matchId != 0) await _fixture.TryDeleteAsync($"/api/matches/{matchId}");
            await _fixture.TryDeleteAsync($"/api/clubs/{homeId}");
            await _fixture.TryDeleteAsync($"/api/clubs/{awayId}");
            await _fixture.TryDeleteAsync($"/api/leagues/{leagueId}");
        }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/matches/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/matches/{missing}", new() { DataObject = new { leagueId = 1, homeTeamId = 1, awayTeamId = 2, date = "2026-01-01T00:00:00", homeGoals = 0, awayGoals = 0 } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/matches/{missing}")).Status);
    }

    [Fact]
    public async Task Post_UnknownLeague_Returns400()
    {
        var resp = await Api.PostAsync("/api/matches", new()
        {
            DataObject = new { leagueId = 999999, homeTeamId = 1, awayTeamId = 2, date = "2026-01-01T00:00:00", homeGoals = 0, awayGoals = 0 }
        });
        Assert.Equal(400, resp.Status);
    }
}
