using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class MatchDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public LeagueSummaryDTO League { get; set; } = null!;
        public ClubSummaryDTO HomeTeam { get; set; } = null!;
        public ClubSummaryDTO AwayTeam { get; set; } = null!;
    }

    public class MatchRequest
    {
        [Required]
        public DateTime Date { get; set; }

        [Range(0, 99)]
        public int HomeGoals { get; set; }

        [Range(0, 99)]
        public int AwayGoals { get; set; }

        [Required]
        public int? LeagueId { get; set; }

        [Required]
        public int? HomeTeamId { get; set; }

        [Required]
        public int? AwayTeamId { get; set; }
    }
}
