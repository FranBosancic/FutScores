using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class LeaguesApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public LeaguesApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/leagues");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }

    [Fact]
    public async Task Crud_Roundtrip()
    {
        int id = 0;
        try
        {
            var create = await Api.PostAsync("/api/leagues", new() { DataObject = new { name = $"PW League {ApiFixture.Tag()}" } });
            Assert.Equal(201, create.Status);
            id = await create.IdAsync();

            Assert.Equal(200, (await Api.GetAsync($"/api/leagues/{id}")).Status);

            var newName = $"PW League Updated {ApiFixture.Tag()}";
            var put = await Api.PutAsync($"/api/leagues/{id}", new() { DataObject = new { name = newName } });
            Assert.Equal(200, put.Status);
            Assert.Equal(newName, (await put.JsonElementAsync()).GetProperty("name").GetString());

            Assert.Equal(204, (await Api.DeleteAsync($"/api/leagues/{id}")).Status);
            Assert.Equal(404, (await Api.GetAsync($"/api/leagues/{id}")).Status);
            id = 0;
        }
        finally { if (id != 0) await _fixture.TryDeleteAsync($"/api/leagues/{id}"); }
    }

    [Fact]
    public async Task MissingId_Returns404()
    {
        const int missing = 999999;
        Assert.Equal(404, (await Api.GetAsync($"/api/leagues/{missing}")).Status);
        Assert.Equal(404, (await Api.PutAsync($"/api/leagues/{missing}", new() { DataObject = new { name = "x" } })).Status);
        Assert.Equal(404, (await Api.DeleteAsync($"/api/leagues/{missing}")).Status);
    }

    [Fact]
    public async Task Post_InvalidInput_Returns400()
    {
        var resp = await Api.PostAsync("/api/leagues", new() { DataObject = new { name = "" } });
        Assert.Equal(400, resp.Status);
    }
}
