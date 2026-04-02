namespace Proba.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public int FoundationYear { get; set; }
        public required string Stadium { get; set; }
        public int Points { get; set; }
        public List<Player> Players { get; set; }
        public int LeagueId { get; set; }
        public required League League { get; set; }

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
