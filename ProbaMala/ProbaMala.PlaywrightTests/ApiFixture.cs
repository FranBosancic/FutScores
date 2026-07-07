using System.Text.Json;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Shared Playwright setup: one API request context authenticated as the seeded admin,
// created once and reused by every test class (via the [Collection("api")] attribute).
// The tests run against a RUNNING FutScores app (default http://localhost:5009), so every
// test cleans up whatever it creates.
public class ApiFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;

    public IAPIRequestContext Api { get; private set; } = null!;   // admin-authenticated
    public string BaseUrl { get; } =
        Environment.GetEnvironmentVariable("FUTSCORES_URL") ?? "http://localhost:5009";
    public string Token { get; private set; } = "";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        // Step 0 of everything: obtain a JWT via the auth endpoint (anonymous request).
        var anon = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true
        });

        var login = await anon.PostAsync("/api/auth/token", new()
        {
            DataObject = new { email = "admin@futscores.local", password = "Admin123!" }
        });

        if (login.Status != 200)
            throw new Exception(
                $"Login failed ({login.Status}). Is the FutScores app running at {BaseUrl}? " +
                await login.TextAsync());

        Token = (await login.JsonAsync())!.Value.GetProperty("token").GetString()!;
        await anon.DisposeAsync();

        // Main context: sends the admin bearer token on every request.
        Api = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {Token}"
            }
        });
    }

    public async Task DisposeAsync()
    {
        if (Api is not null) await Api.DisposeAsync();
        _playwright?.Dispose();
    }

    // A no-auth context, for negative authorization tests.
    public Task<IAPIRequestContext> NewAnonymousContextAsync() =>
        _playwright.APIRequest.NewContextAsync(new() { BaseURL = BaseUrl, IgnoreHTTPSErrors = true });

    // ── Seed helpers: create a valid entity graph for tests that need FKs; return ids ──

    public Task<int> CreateLeagueAsync() =>
        PostIdAsync("/api/leagues", new { name = $"PW League {Tag()}" });

    public async Task<(int leagueId, int clubId)> CreateClubAsync()
    {
        int leagueId = await CreateLeagueAsync();
        int clubId = await PostIdAsync("/api/clubs",
            new { name = $"PW Club {Tag()}", foundedDate = "1990-01-01", leagueId });
        return (leagueId, clubId);
    }

    public async Task<(int leagueId, int clubId, int playerId)> CreatePlayerAsync()
    {
        var (leagueId, clubId) = await CreateClubAsync();
        int playerId = await PostIdAsync("/api/players", new
        {
            firstName = "PW", lastName = $"Player{Tag()}", dateOfBirth = "1998-05-03",
            position = 3, nationality = "Testland", clubId   // position 3 = Forward
        });
        return (leagueId, clubId, playerId);
    }

    public async Task<(int leagueId, int homeId, int awayId, int matchId)> CreateMatchAsync()
    {
        int leagueId = await CreateLeagueAsync();
        int homeId = await PostIdAsync("/api/clubs", new { name = $"PW Home {Tag()}", foundedDate = "1990-01-01", leagueId });
        int awayId = await PostIdAsync("/api/clubs", new { name = $"PW Away {Tag()}", foundedDate = "1991-01-01", leagueId });
        int matchId = await PostIdAsync("/api/matches", new
        {
            leagueId, homeTeamId = homeId, awayTeamId = awayId,
            date = "2026-05-10T18:00:00", homeGoals = 2, awayGoals = 1
        });
        return (leagueId, homeId, awayId, matchId);
    }

    public Task<int> CreateUserAsync() =>
        PostIdAsync("/api/users", new { firstName = "PW", lastName = $"User{Tag()}", email = $"pw{Tag()}@example.com" });

    // Best-effort delete used for cleanup — never throws.
    public async Task TryDeleteAsync(string url)
    {
        try { await Api.DeleteAsync(url); } catch { /* best effort */ }
    }

    private async Task<int> PostIdAsync(string url, object body)
    {
        var resp = await Api.PostAsync(url, new() { DataObject = body });
        if (resp.Status is not (200 or 201))
            throw new Exception($"Seed POST {url} failed ({resp.Status}): {await resp.TextAsync()}");
        return (await resp.JsonAsync())!.Value.GetProperty("id").GetInt32();
    }

    // Short unique suffix so seeded names/emails never collide with existing data.
    public static string Tag() => Guid.NewGuid().ToString("N")[..8];
}

[CollectionDefinition("api")]
public class ApiCollection : ICollectionFixture<ApiFixture> { }

// Small conveniences for reading Playwright API responses in the tests.
public static class ApiResponseExtensions
{
    public static async Task<JsonElement> JsonElementAsync(this IAPIResponse response) =>
        (await response.JsonAsync())!.Value;

    public static async Task<int> IdAsync(this IAPIResponse response) =>
        (await response.JsonAsync())!.Value.GetProperty("id").GetInt32();
}
