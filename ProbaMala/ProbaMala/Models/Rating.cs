namespace ProbaMala.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}