namespace Proba.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int JerseyNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public PlayerPosition Position { get; set; }
        public int Appearances { get; set; }
        public int ClubId { get; set; }
        public Club Club { get; set; }
        public List<PlayerMatchRating> MatchRatings { get; set; }

        public Player()
        {
            MatchRatings = new List<PlayerMatchRating>();
        }
    }
}
