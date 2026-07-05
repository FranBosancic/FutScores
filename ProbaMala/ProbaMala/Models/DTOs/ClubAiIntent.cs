namespace ProbaMala.Models.DTOs
{
    // Structured club data the AI extracts from a natural-language note.
    // LeagueName is a name (resolved to a LeagueId by our own code); FoundedDate is
    // parsed in the controller (accepts a year or an ISO date).
    public class ClubAiIntent
    {
        public string Name { get; set; } = "";
        public string? FoundedDate { get; set; }
        public string LeagueName { get; set; } = "";
    }
}
