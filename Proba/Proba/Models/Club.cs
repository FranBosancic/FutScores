namespace Proba.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int FoundationYear { get; set; }
        public string Stadium { get; set; } = string.Empty;
        public int Points { get; set; }
        public List<Player> Players { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; }

        public Club()
        {
            Players = new List<Player>();
        }

        public Club(string name, string city, int foundationYear, string stadium, League league)
        {
            Name = name;
            City = city;
            FoundationYear = foundationYear;
            Stadium = stadium;
            League = league;
            Points = 0;
            Players = new List<Player>();
        }
    }
}
