using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class ClubDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime FoundedDate { get; set; }
        public LeagueSummaryDTO League { get; set; } = null!;
        public int PlayerCount { get; set; }
        public int MatchCount { get; set; }
    }

    public class ClubRequest
    {
        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = null!;

        [Required]
        public DateTime FoundedDate { get; set; }

        [Required]
        public int? LeagueId { get; set; }
    }
}
