using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class LeagueDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ClubCount { get; set; }
        public int MatchCount { get; set; }
    }

    public class LeagueRequest
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = null!;
    }
}
