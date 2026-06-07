namespace ProbaMala.Models.ViewModels
{
    public class MatchDetailsViewModel
    {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public DateTime Date { get; set; }
        public string KickoffLabel { get; set; } = null!;
        public string StatusLabel { get; set; } = null!;
        public string StatusTone { get; set; } = null!;
        public string LeagueName { get; set; } = null!;
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public int RatingCount { get; set; }

        public string Score => $"{HomeGoals} - {AwayGoals}";

        // ── Match stats (only populated on the Details page) ──
        public double? AverageRating { get; set; }
        public double? HomeAverageRating { get; set; }
        public double? AwayAverageRating { get; set; }
        public string? TopRatedPlayerName { get; set; }
        public double? TopRatedPlayerScore { get; set; }

        // Side-by-side squads, with per-player ratings for this match.
        public List<MatchSquadPlayerViewModel>? HomeSquad { get; set; }
        public List<MatchSquadPlayerViewModel>? AwaySquad { get; set; }

        // Only populated on the Details page (null on the Index list).
        public List<RatingDetailsViewModel>? Ratings { get; set; }
    }
}