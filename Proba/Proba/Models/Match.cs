namespace Proba.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int HomeClubId { get; set; }
        public required Club HomeClub { get; set; }
        public int AwayClubId { get; set; }
        public required Club AwayClub { get; set; }
        public DateTime MatchDate { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int LeagueId { get; set; }
        public required League League { get; set; }
        public List<PlayerMatchRating> PlayerRatings { get; set; }
        public MatchRating MatchRating { get; set; }

        public Match()
        {
            PlayerRatings = new List<PlayerMatchRating>();
        }

        public Match(Club homeClub, Club awayClub, DateTime matchDate, int homeScore, int awayScore, League league)
        {
            HomeClub = homeClub;
            HomeClubId = homeClub.ClubId;
            AwayClub = awayClub;
            AwayClubId = awayClub.ClubId;
            MatchDate = matchDate;
            HomeScore = homeScore;
            AwayScore = awayScore;
            League = league;
            LeagueId = league.LeagueId;
            PlayerRatings = new List<PlayerMatchRating>();
        }
    }
}
