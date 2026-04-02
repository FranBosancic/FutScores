namespace Proba.Models
{
    public class MatchRating
    {
        public int MatchRatingId { get; set; }
        public int MatchId { get; set; }
        public required Match Match { get; set; }
        public int Rating { get; set; }
        public required string Comments { get; set; }
        public DateTime RatedAt { get; set; }
        public required string RatedBy { get; set; }

        public MatchRating()
        {
        }

        public MatchRating(Match match, int rating, string comments, string ratedBy, DateTime ratedAt)
        {
            Match = match;
            MatchId = match.MatchId;
            Rating = rating;
            Comments = comments;
            RatedBy = ratedBy;
            RatedAt = ratedAt;
        }
    }
}
