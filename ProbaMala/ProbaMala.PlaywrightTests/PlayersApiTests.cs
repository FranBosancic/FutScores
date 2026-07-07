using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class PlayersApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public PlayersApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/players");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        var (leagueId, clubId) = await _fixture.CreateClubAsync();
        int playerId = 0;
        var tag = ApiFixture.Tag();
        try
        {
            var create = await Api.PostAsync("/api/players", new()
            {
                DataObject = new { firstName = "PW", lastName = $"Player{tag}", dateOfBirth = "1998-05-03", position = 3, nationality = "Testland", clubId }
            });
            Assert.Equal(201, create.Status);
            playerId = await create.IdAsync();

            Assert.Equal(200, (await Api.GetAsync($"/api/players/{playerId}")).Status);

            var put = await Api.PutAsync($"/api/players/{playerId}", new()
            {
                DataObject = new { firstName = "PW", lastName = $"Renamed{tag}", dateOfBirth = "1998-05-03", position = 2, nationality = "Testland", clubId }
            });
            Assert.Equal(200, put.Status);

            Assert.Equal(204, (await Api.DeleteAsync($"/api/players/{playerId}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/players/{playerId}")).Status);
            playerId = 0;
        }
        finally
        {
            if (playerId != 0) await _fixture.TryDeleteAsync($"/api/players/{playerId}");
            await _fixture.TryDeleteAsync($"/api/clubs/{clubId}");
            await _fixture.TryDeleteAsync($"/api/leagues/{leagueId}");
        }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/players/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/players/{missing}", new() { DataObject = new { firstName = "x", lastName = "y", dateOfBirth = "2000-01-01", position = 1, nationality = "z", clubId = 1 } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/players/{missing}")).Status);
    }

    [Fact]
    public async Task Post_UnknownClub_Returns400()
    {
        var resp = await Api.PostAsync("/api/players", new()
        {
            DataObject = new { firstName = "PW", lastName = $"NoClub{ApiFixture.Tag()}", dateOfBirth = "2000-01-01", position = 1, nationality = "Testland", clubId = 999999 }
        });
        Assert.Equal(400, resp.Status);
    }
}
