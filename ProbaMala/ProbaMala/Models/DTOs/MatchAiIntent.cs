namespace ProbaMala.Models.DTOs
{
    // Structured match data the AI extracts from a natural-language note. The club names
    // are resolved to ids by our own code (the league is derived from the home club), and
    // Date is parsed in the controller.
    public class MatchAiIntent
    {
        public string HomeTeamName { get; set; } = "";
        public string AwayTeamName { get; set; } = "";
        public string? Date { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
    }
}
