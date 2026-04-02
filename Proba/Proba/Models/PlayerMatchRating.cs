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
        public string Performance { get; set; } = string.Empty;
        public DateTime RatedAt { get; set; }

        public PlayerMatchRating()
        {
        }

        public PlayerMatchRating(Player player, Match match, int rating, string performance, DateTime ratedAt)
        {
            Player = player;
            PlayerId = player.PlayerId;
            Match = match;
            MatchId = match.MatchId;
            Rating = rating;
            Performance = performance;
            RatedAt = ratedAt;
        }
    }
}
