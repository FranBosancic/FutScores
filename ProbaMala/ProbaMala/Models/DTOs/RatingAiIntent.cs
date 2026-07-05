namespace ProbaMala.Models.DTOs
{
    // The structured shape the AI extracts from a natural-language rating note.
    // It carries *names*, not database ids — the controller resolves those against
    // the DB so our own code stays the authority for what actually exists.
    public class RatingAiIntent
    {
        public string PlayerName { get; set; } = "";
        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
