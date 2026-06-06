namespace ProbaMala.Models.ViewModels
{
    public class LeagueDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ClubCount { get; set; }
        public int MatchCount { get; set; }

        // Only populated on the Details page (null on the Index list).
        // Keeping them on the same VM avoids a separate class for one extra query.
        public List<ClubDetailsViewModel>? Clubs { get; set; }
        public List<MatchDetailsViewModel>? Matches { get; set; }
    }
}
