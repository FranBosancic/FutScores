namespace Proba.Models
{
    public class League
    {
        public int LeagueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int FoundationYear { get; set; }
        public int TotalClubs { get; set; }
        public List<Club> Clubs { get; set; }
        public List<Match> Matches { get; set; }

        public League()
        {
            Clubs = new List<Club>();
            Matches = new List<Match>();
        }

        public League(string name, string country, int foundationYear, int totalClubs)
        {
            Name = name;
            Country = country;
            FoundationYear = foundationYear;
            TotalClubs = totalClubs;
            Clubs = new List<Club>();
            Matches = new List<Match>();
        }
    }
}
