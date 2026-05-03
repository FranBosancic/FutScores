using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbaMala.Models.Entities
{
    public class Club
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = null!;

        public DateTime FoundedDate { get; set; }

        [ForeignKey(nameof(League))]
        public int LeagueId { get; set; }

        public virtual League League { get; set; } = null!;

        public virtual ICollection<Player> Players { get; set; } = new List<Player>();

        [InverseProperty(nameof(Match.HomeTeam))]
        public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();

        [InverseProperty(nameof(Match.AwayTeam))]
        public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}