using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbaMala.Models.Entities
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }

        [ForeignKey(nameof(Match))]
        public int MatchId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }

        [MaxLength(400)]
        public string? Comment { get; set; }

        public virtual Player Player { get; set; } = null!;
        public virtual Match Match { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}