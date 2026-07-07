using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class ClubsApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public ClubsApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/clubs");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        int leagueId = await _fixture.CreateLeagueAsync();
        int clubId = 0;
        var tag = ApiFixture.Tag();
        try
        {
            var create = await Api.PostAsync("/api/clubs", new() { DataObject = new { name = $"PW Club {tag}", foundedDate = "1980-01-01", leagueId } });
            Assert.Equal(201, create.Status);
            clubId = await create.IdAsync();

            Assert.Equal(200, (await Api.GetAsync($"/api/clubs/{clubId}")).Status);

            var put = await Api.PutAsync($"/api/clubs/{clubId}", new() { DataObject = new { name = $"PW Club Updated {tag}", foundedDate = "1985-01-01", leagueId } });
            Assert.Equal(200, put.Status);

            Assert.Equal(204, (await Api.DeleteAsync($"/api/clubs/{clubId}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/clubs/{clubId}")).Status);
            clubId = 0;
        }
        finally
        {
            if (clubId != 0) await _fixture.TryDeleteAsync($"/api/clubs/{clubId}");
            await _fixture.TryDeleteAsync($"/api/leagues/{leagueId}");
        }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/clubs/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/clubs/{missing}", new() { DataObject = new { name = "x", foundedDate = "2000-01-01", leagueId = 1 } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/clubs/{missing}")).Status);
    }

    [Fact]
    public async Task Post_UnknownLeague_Returns400()
    {
        var resp = await Api.PostAsync("/api/clubs", new() { DataObject = new { name = $"PW {ApiFixture.Tag()}", foundedDate = "1990-01-01", leagueId = 999999 } });
        Assert.Equal(400, resp.Status);
    }
}
