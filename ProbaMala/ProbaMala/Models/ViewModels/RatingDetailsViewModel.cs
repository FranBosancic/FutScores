namespace ProbaMala.Models.ViewModels
{
    public class RatingDetailsViewModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public string PlayerName { get; set; } = null!;
        public string MatchDescription { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Score { get; set; }
        public string? Comment { get; set; }

        // Match context, surfaced so lists can show the fixture and scoreline
        // directly instead of a single packed description string.
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime MatchDate { get; set; }

        public string MatchScore => $"{HomeGoals}–{AwayGoals}";
    }
}