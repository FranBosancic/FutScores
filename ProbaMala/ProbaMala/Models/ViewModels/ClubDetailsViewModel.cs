namespace ProbaMala.Models.ViewModels
{
    public class ClubDetailsViewModel
    {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime FoundedDate { get; set; }
        public string LeagueName { get; set; } = null!;
        public int PlayerCount { get; set; }
        public int MatchCount { get; set; }
        public bool CanDelete { get; set; }
    }
}