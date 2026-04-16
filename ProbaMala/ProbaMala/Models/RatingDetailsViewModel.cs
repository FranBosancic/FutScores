namespace ProbaMala.Models
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
    }
}