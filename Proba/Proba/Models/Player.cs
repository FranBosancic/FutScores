namespace Proba.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
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

        public Player(string name, int jerseyNumber, DateTime dateOfBirth, PlayerPosition position, Club club)
        {
            Name = name;
            JerseyNumber = jerseyNumber;
            DateOfBirth = dateOfBirth;
            Position = position;
            Club = club;
            Appearances = 0;
            MatchRatings = new List<PlayerMatchRating>();
        }
    }
}
