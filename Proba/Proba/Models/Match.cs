namespace Proba.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int HomeClubId { get; set; }
        public Club HomeClub { get; set; }
        public int AwayClubId { get; set; }
        public Club AwayClub { get; set; }
        public DateTime MatchDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; }
        public List<PlayerMatchRating> PlayerRatings { get; set; }
        public MatchRating MatchRating { get; set; }

        public Match()
        {
            PlayerRatings = new List<PlayerMatchRating>();
        }
    }
}
