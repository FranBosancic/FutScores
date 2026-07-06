using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ProbaMala.Mcp;

// Thin HTTP client over the FutScores REST API. Every MCP tool goes through here, so all
// the business logic, validation and auth stay in the web app — this project just
// translates tool calls into HTTP requests and returns the JSON responses.
public class FutScoresApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private string? _token;

    public FutScoresApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    // ── Reads: public GET endpoints, no auth ──

    public Task<string> SearchAsync(string? query) => GetAsync($"/api/search?q={Enc(query)}");

    public Task<string> ListLeaguesAsync() => GetAsync("/api/leagues");

    public Task<string> ListClubsAsync(int? leagueId) =>
        GetAsync("/api/clubs" + (leagueId is int l ? $"?leagueId={l}" : ""));

    public Task<string> ListPlayersAsync(string? query, int? clubId)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(query)) parts.Add($"q={Enc(query)}");
        if (clubId is int c) parts.Add($"clubId={c}");
        return GetAsync("/api/players" + (parts.Count > 0 ? "?" + string.Join("&", parts) : ""));
    }

    public Task<string> GetPlayerAsync(int id) => GetAsync($"/api/players/{id}");

    public Task<string> ListMatchesAsync(int? leagueId) =>
        GetAsync("/api/matches" + (leagueId is int l ? $"?leagueId={l}" : ""));

    public Task<string> GetMatchAsync(int id) => GetAsync($"/api/matches/{id}");

    public Task<string> ListRatingsAsync(string? query) => GetAsync($"/api/ratings?q={Enc(query)}");

    public Task<string> ListUsersAsync(string? query) => GetAsync($"/api/users?q={Enc(query)}");

    // ── Write: needs a JWT (obtained from the admin account) ──

    public async Task<string> AddRatingAsync(int playerId, int matchId, int userId, int score, string? comment)
    {
        var token = await GetTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ratings")
        {
            Content = JsonContent.Create(new { playerId, matchId, userId, score, comment })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode ? body : $"Error {(int)response.StatusCode}: {body}";
    }

    // Fetches (and caches) a JWT via POST /api/auth/token using the admin credentials
    // from config (falling back to the dev seed account).
    private async Task<string> GetTokenAsync()
    {
        if (_token != null)
            return _token;

        var email    = _config["FutScores:Admin:Email"]    ?? "admin@futscores.local";
        var password = _config["FutScores:Admin:Password"] ?? "Admin123!";

        var response = await _http.PostAsJsonAsync("/api/auth/token", new { email, password });
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        _token = doc.RootElement.GetProperty("token").GetString();
        return _token!;
    }

    private async Task<string> GetAsync(string path)
    {
        var response = await _http.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode ? body : $"Error {(int)response.StatusCode}: {body}";
    }

    private static string Enc(string? value) => Uri.EscapeDataString(value ?? "");
}
