namespace ProbaMala.Models
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

        public string Score => $"{HomeGoals} - {AwayGoals}";
    }
}