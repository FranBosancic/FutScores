namespace Proba.Models
{
    public class PlayerMatchRating
    {
        public int PlayerMatchRatingId { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public int Rating { get; set; }
        public string Performance { get; set; }
        public DateTime RatedAt { get; set; }
    }
}
