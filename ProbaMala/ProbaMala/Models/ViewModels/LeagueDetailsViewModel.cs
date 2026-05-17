namespace ProbaMala.Models.ViewModels
{
    public class LeagueDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ClubCount { get; set; }
        public int MatchCount { get; set; }
    }
}