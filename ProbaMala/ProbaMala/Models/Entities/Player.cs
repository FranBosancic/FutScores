using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbaMala.Models.Entities
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string LastName { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public Position Position { get; set; }

        [ForeignKey(nameof(Club))]
        public int ClubId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Nationality { get; set; } = null!;

        public virtual Club Club { get; set; } = null!;
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}