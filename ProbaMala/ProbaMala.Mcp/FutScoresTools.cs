using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ProbaMala.Mcp;

// The tools exposed over MCP. Each method is one tool the agentic IDE can call; the
// [Description] text is what the model reads to decide when to use it. All of them
// delegate to FutScoresApiClient, which calls the FutScores REST API.
[McpServerToolType]
public class FutScoresTools
{
    private readonly FutScoresApiClient _api;

    public FutScoresTools(FutScoresApiClient api)
    {
        _api = api;
    }

    [McpServerTool, Description("Search FutScores across pages, leagues, clubs, players, matches, ratings and users. Returns matching results as JSON.")]
    public Task<string> Search([Description("Free-text search query")] string query)
        => _api.SearchAsync(query);

    [McpServerTool, Description("List all football leagues.")]
    public Task<string> ListLeagues()
        => _api.ListLeaguesAsync();

    [McpServerTool, Description("List clubs, optionally filtered to one league.")]
    public Task<string> ListClubs([Description("Optional league id to filter by")] int? leagueId = null)
        => _api.ListClubsAsync(leagueId);

    [McpServerTool, Description("List players, optionally filtered by a name/nationality query and/or a club id.")]
    public Task<string> ListPlayers(
        [Description("Optional name or nationality search")] string? query = null,
        [Description("Optional club id to filter by")] int? clubId = null)
        => _api.ListPlayersAsync(query, clubId);

    [McpServerTool, Description("Get a single player by id, including their club.")]
    public Task<string> GetPlayer([Description("Player id")] int id)
        => _api.GetPlayerAsync(id);

    [McpServerTool, Description("List matches, optionally filtered to one league.")]
    public Task<string> ListMatches([Description("Optional league id to filter by")] int? leagueId = null)
        => _api.ListMatchesAsync(leagueId);

    [McpServerTool, Description("Get a single match by id, including both clubs and the score.")]
    public Task<string> GetMatch([Description("Match id")] int id)
        => _api.GetMatchAsync(id);

    [McpServerTool, Description("List ratings, optionally filtered by a text query (player, club, comment or score).")]
    public Task<string> ListRatings([Description("Optional search query")] string? query = null)
        => _api.ListRatingsAsync(query);

    [McpServerTool, Description("List rating authors (users), optionally filtered by a text query. Use this to find a userId for add_rating.")]
    public Task<string> ListUsers([Description("Optional search query")] string? query = null)
        => _api.ListUsersAsync(query);

    [McpServerTool, Description("Add a player rating for a match. Provide the playerId, matchId, an author userId (from list_users), and a score from 1 to 10; comment is optional.")]
    public Task<string> AddRating(
        [Description("Id of the player being rated")] int playerId,
        [Description("Id of the match")] int matchId,
        [Description("Id of the rating author (see list_users)")] int userId,
        [Description("Score from 1 to 10")] int score,
        [Description("Optional short comment")] string? comment = null)
        => _api.AddRatingAsync(playerId, matchId, userId, score, comment);
}
