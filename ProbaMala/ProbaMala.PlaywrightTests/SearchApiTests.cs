using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class SearchApiTests
{
    private readonly ApiFixture _fixture;
    private IAPIRequestContext Api => _fixture.Api;

    public SearchApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Search_ReturnsMatchingResults()
    {
        // "arsenal" matches the seeded club (and related players/matches).
        var resp = await Api.GetAsync("/api/search?q=arsenal");
        Assert.Equal(200, resp.Status);

        var results = await resp.JsonElementAsync();
        Assert.Equal(JsonValueKind.Array, results.ValueKind);
        Assert.True(results.GetArrayLength() > 0, "expected at least one result for 'arsenal'");
    }

    [Fact]
    public async Task Search_EmptyQuery_ReturnsOk()
    {
        var resp = await Api.GetAsync("/api/search");
        Assert.Equal(200, resp.Status);
        Assert.Equal(JsonValueKind.Array, (await resp.JsonElementAsync()).ValueKind);
    }
}
