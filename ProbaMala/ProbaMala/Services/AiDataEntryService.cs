using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using ProbaMala.Models.DTOs;

namespace ProbaMala.Services
{
    // AI-assisted data entry: turns a natural-language note into a structured intent using
    // Claude's structured-output mode. The AI only does language understanding — resolving
    // names to database ids and validating the result stays in our own code (the controllers
    // + INameResolver). Behind an interface so the app runs (feature disabled) when no API
    // key is configured, and so tests never make a network call.
    public interface IAiDataEntryService
    {
        // False when no API key is configured — the UI hides the AI box in that case.
        bool IsConfigured { get; }

        Task<AiExtractionResult<RatingAiIntent>> ExtractRatingAsync(string prompt);
        Task<AiExtractionResult<PlayerAiIntent>> ExtractPlayerAsync(string prompt);
        Task<AiExtractionResult<ClubAiIntent>>   ExtractClubAsync(string prompt);
        Task<AiExtractionResult<MatchAiIntent>>  ExtractMatchAsync(string prompt);
        Task<AiExtractionResult<UserAiIntent>>   ExtractUserAsync(string prompt);
    }

    // A tiny result wrapper: either a parsed value or a user-facing error message.
    public record AiExtractionResult<T>(bool Success, T? Value, string? Error)
    {
        public static AiExtractionResult<T> Ok(T value) => new(true, value, null);
        public static AiExtractionResult<T> Fail(string error) => new(false, default, error);
    }

    public class AiDataEntryService : IAiDataEntryService
    {
        private readonly string? _apiKey;
        private readonly string _model;
        private readonly ILogger<AiDataEntryService> _logger;

        public AiDataEntryService(IConfiguration configuration, ILogger<AiDataEntryService> logger)
        {
            // Never hard-coded: read from user-secrets (dev) or an environment variable
            // (prod), same pattern as the Jwt key and Google credentials.
            _apiKey = configuration["Ai:ApiKey"];
            _model  = configuration["Ai:Model"] ?? "claude-opus-4-8";
            _logger = logger;
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

        // ── Per-entity extraction ─────────────────────────────────────────────
        // Each method supplies a system prompt + JSON schema and delegates the actual
        // Claude call/parse to the shared ExtractAsync<T> below.

        public Task<AiExtractionResult<RatingAiIntent>> ExtractRatingAsync(string prompt) =>
            ExtractAsync<RatingAiIntent>(prompt,
                "You extract structured football player-rating data from a short note. Identify the player " +
                "being rated, the two clubs that played the match (use each club's full common name — e.g. " +
                "\"Manchester City\", not \"Man City\"), a score from 1 (very poor) to 10 (outstanding), and an " +
                "optional short comment. If the note is vague, make your best single guess.",
                Schema(new
                {
                    playerName   = new { type = "string", description = "Full name of the player being rated." },
                    homeTeamName = new { type = "string", description = "Full common name of the home club." },
                    awayTeamName = new { type = "string", description = "Full common name of the away club." },
                    score        = new { type = "integer", description = "Rating from 1 to 10." },
                    comment      = new { type = "string", description = "Optional short comment; empty string if none." }
                }, "playerName", "homeTeamName", "awayTeamName", "score"));

        public Task<AiExtractionResult<PlayerAiIntent>> ExtractPlayerAsync(string prompt) =>
            ExtractAsync<PlayerAiIntent>(prompt,
                "You extract a football player profile from a short note. Give the first and last name, date of " +
                "birth as an ISO date (YYYY-MM-DD; best estimate if only a year is given), the playing position " +
                "(one of Goalkeeper, Defender, Midfielder, Forward), nationality, and the club's full common name.",
                Schema(new
                {
                    firstName   = new { type = "string" },
                    lastName    = new { type = "string" },
                    dateOfBirth = new { type = "string", description = "ISO date YYYY-MM-DD." },
                    position    = new { type = "string", @enum = new[] { "Goalkeeper", "Defender", "Midfielder", "Forward" } },
                    nationality = new { type = "string" },
                    clubName    = new { type = "string", description = "Full common name of the player's club." }
                }, "firstName", "lastName", "position", "nationality", "clubName"));

        public Task<AiExtractionResult<ClubAiIntent>> ExtractClubAsync(string prompt) =>
            ExtractAsync<ClubAiIntent>(prompt,
                "You extract football club data from a short note. Give the club name, the founding date as a year " +
                "or an ISO date, and the full common name of the league/competition it plays in.",
                Schema(new
                {
                    name        = new { type = "string" },
                    foundedDate = new { type = "string", description = "Founding year or ISO date." },
                    leagueName  = new { type = "string", description = "Full common name of the league." }
                }, "name", "leagueName"));

        public Task<AiExtractionResult<MatchAiIntent>> ExtractMatchAsync(string prompt) =>
            ExtractAsync<MatchAiIntent>(prompt,
                "You extract football match data from a short note. Give the two clubs by their full common name, " +
                "the kickoff date as an ISO date, and the final score (home goals and away goals). Home is the team " +
                "listed first or described as playing at home; if unclear, use the first team mentioned as home.",
                Schema(new
                {
                    homeTeamName = new { type = "string", description = "Full common name of the home club." },
                    awayTeamName = new { type = "string", description = "Full common name of the away club." },
                    date         = new { type = "string", description = "Kickoff date, ISO YYYY-MM-DD." },
                    homeGoals    = new { type = "integer", description = "Goals scored by the home club." },
                    awayGoals    = new { type = "integer", description = "Goals scored by the away club." }
                }, "homeTeamName", "awayTeamName"));

        public Task<AiExtractionResult<UserAiIntent>> ExtractUserAsync(string prompt) =>
            ExtractAsync<UserAiIntent>(prompt,
                "You extract a person's profile from a short note: their first name, last name, and email address.",
                Schema(new
                {
                    firstName = new { type = "string" },
                    lastName  = new { type = "string" },
                    email     = new { type = "string", description = "Email address." }
                }, "firstName", "lastName", "email"));

        // ── Shared plumbing ───────────────────────────────────────────────────

        private async Task<AiExtractionResult<T>> ExtractAsync<T>(
            string prompt, string systemPrompt, Dictionary<string, JsonElement> schema)
        {
            if (!IsConfigured)
                return AiExtractionResult<T>.Fail("The AI assistant is not configured.");

            try
            {
                AnthropicClient client = new() { ApiKey = _apiKey };

                var parameters = new MessageCreateParams
                {
                    Model     = _model,
                    MaxTokens = 1024,
                    System    = systemPrompt,
                    Messages  = [new() { Role = Role.User, Content = prompt }],
                    // JSON-schema-constrained output — the reply is guaranteed to match
                    // the schema, so we can deserialize it without brittle parsing.
                    OutputConfig = new OutputConfig { Format = new JsonOutputFormat { Schema = schema } }
                };

                var response = await client.Messages.Create(parameters);

                if (response.StopReason == "refusal")
                    return AiExtractionResult<T>.Fail("The AI declined to process that request.");

                var json = response.Content
                    .Select(block => block.Value)
                    .OfType<TextBlock>()
                    .FirstOrDefault()?
                    .Text;

                if (string.IsNullOrWhiteSpace(json))
                    return AiExtractionResult<T>.Fail("The AI returned an empty response. Try rephrasing.");

                var value = JsonSerializer.Deserialize<T>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (value is null)
                    return AiExtractionResult<T>.Fail("The AI couldn't produce a usable result. Try rephrasing.");

                return AiExtractionResult<T>.Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI extraction failed for {Type}.", typeof(T).Name);
                return AiExtractionResult<T>.Fail(
                    "The AI request failed. Please try again or fill in the form manually.");
            }
        }

        // Builds a minimal JSON-schema object for the given properties (all strings/ints)
        // with `additionalProperties: false` so the model can't invent extra fields.
        private static Dictionary<string, JsonElement> Schema(object properties, params string[] required) => new()
        {
            ["type"]                 = JsonSerializer.SerializeToElement("object"),
            ["properties"]           = JsonSerializer.SerializeToElement(properties),
            ["required"]             = JsonSerializer.SerializeToElement(required),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false)
        };
    }
}
