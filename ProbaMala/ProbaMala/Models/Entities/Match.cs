using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbaMala.Models.Entities
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(League))]
        public int LeagueId { get; set; }

        [ForeignKey(nameof(HomeTeam))]
        public int HomeTeamId { get; set; }

        [ForeignKey(nameof(AwayTeam))]
        public int AwayTeamId { get; set; }

        public DateTime Date { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }

        public virtual League League { get; set; } = null!;

        [InverseProperty(nameof(Club.HomeMatches))]
        public virtual Club HomeTeam { get; set; } = null!;

        [InverseProperty(nameof(Club.AwayMatches))]
        public virtual Club AwayTeam { get; set; } = null!;

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}