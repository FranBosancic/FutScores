using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

[Collection("api")]
public class AuthApiTests
{
    private readonly ApiFixture _fixture;

    public AuthApiTests(ApiFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Token_ValidCredentials_ReturnsToken()
    {
        var anon = await _fixture.NewAnonymousContextAsync();
        try
        {
            var resp = await anon.PostAsync("/api/auth/token", new() { DataObject = new { email = "admin@futscores.local", password = "Admin123!" } });
            Assert.Equal(200, resp.Status);
            Assert.False(string.IsNullOrEmpty((await resp.JsonElementAsync()).GetProperty("token").GetString()));
        }
        finally { await anon.DisposeAsync(); }
    }

    [Fact]
    public async Task Token_BadCredentials_ReturnsUnauthorized()
    {
        var anon = await _fixture.NewAnonymousContextAsync();
        try
        {
            var resp = await anon.PostAsync("/api/auth/token", new() { DataObject = new { email = "nobody@example.com", password = "wrong" } });
            Assert.Equal(401, resp.Status);
        }
        finally { await anon.DisposeAsync(); }
    }

    [Fact]
    public async Task Mutation_WithInvalidToken_ReturnsUnauthorized()
    {
        var anon = await _fixture.NewAnonymousContextAsync();
        try
        {
            var resp = await anon.PostAsync("/api/leagues", new()
            {
                DataObject = new { name = "should-not-be-created" },
                Headers = new Dictionary<string, string> { ["Authorization"] = "Bearer not-a-real-token" }
            });
            Assert.Equal(401, resp.Status);
        }
        finally { await anon.DisposeAsync(); }
    }
}
