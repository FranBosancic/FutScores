using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class RatingDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public PlayerSummaryDTO Player { get; set; } = null!;
        public MatchSummaryDTO Match { get; set; } = null!;
        public UserSummaryDTO User { get; set; } = null!;
    }

    public class RatingRequest
    {
        [Required]
        public int? PlayerId { get; set; }

        [Required]
        public int? MatchId { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }

        [MaxLength(400)]
        public string? Comment { get; set; }
    }
}
