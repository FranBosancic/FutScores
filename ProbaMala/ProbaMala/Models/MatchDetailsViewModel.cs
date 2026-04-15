namespace ProbaMala.Models
{
    public class MatchDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string LeagueName { get; set; } = null!;
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }

        public string Score => $"{HomeGoals} - {AwayGoals}";
    }
}