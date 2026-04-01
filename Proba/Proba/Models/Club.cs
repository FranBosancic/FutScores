namespace Proba.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int FoundationYear { get; set; }
        public string Stadium { get; set; }
        public int Points { get; set; }
        public List<Player> Players { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; }

        public Club()
        {
            Players = new List<Player>();
        }
    }
}
