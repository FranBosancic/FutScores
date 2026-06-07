using ProbaMala.Models.Entities;

namespace ProbaMala.Models.ViewModels
{
    /// <summary>
    /// A single player as shown in the side-by-side squad table on the match
    /// details page, carrying that player's rating aggregates for this match.
    /// </summary>
    public class MatchSquadPlayerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public Position Position { get; set; }

        // Ratings this player received in this specific match.
        public int RatingCount { get; set; }
        public double? AverageScore { get; set; }

        // Highest-rated player in the match — highlighted as "player of the match".
        public bool IsTopRated { get; set; }
    }
}
