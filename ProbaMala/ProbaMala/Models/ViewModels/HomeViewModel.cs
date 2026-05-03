using ProbaMala.Models.Entities;

namespace ProbaMala.Models.ViewModels
{
    public class DashboardMatchCard
    {
        public int MatchId { get; set; }
        public string LeagueName { get; set; } = null!;
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime Kickoff { get; set; }
        public string KickoffLabel { get; set; } = null!;
        public string StatusLabel { get; set; } = null!;
        public string StatusTone { get; set; } = null!;
    }

    public class DashboardFeaturedPlayer
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = null!;
        public string ClubName { get; set; } = null!;
        public Position Position { get; set; }
        public string Nationality { get; set; } = null!;
        public double AverageRating { get; set; }
    }

    public class DashboardSearchShortcut
    {
        public string Label { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string RouteName { get; set; } = null!;
    }

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
        public List<DashboardMatchCard> RecentMatches { get; set; } = new();
        public List<DashboardFeaturedPlayer> FeaturedPlayers { get; set; } = new();
        public List<DashboardSearchShortcut> SearchShortcuts { get; set; } = new();
        public List<ClubFormEntry> ClubForms { get; set; } = new();
    }
}