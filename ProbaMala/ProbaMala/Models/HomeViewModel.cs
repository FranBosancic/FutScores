namespace ProbaMala.Models
{
    public class ClubFormEntry
    {
        public string ClubName { get; set; } = null!;
        public List<string> Results { get; set; } = new();
    }

    public class HomeViewModel
    {
        public int TotalClubs { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalMatches { get; set; }
        public double AverageRating { get; set; }
        public List<ClubFormEntry> ClubForms { get; set; } = new();
    }
}