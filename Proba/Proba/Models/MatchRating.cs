namespace Proba.Models
{
    public class MatchRating
    {
        public int MatchRatingId { get; set; }
        public int MatchId { get; set; }
        public Match Match { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
        public DateTime RatedAt { get; set; }
        public string RatedBy { get; set; }
    }
}
