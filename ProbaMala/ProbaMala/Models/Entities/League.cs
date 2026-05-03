using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.Entities
{
    public class League
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();
        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}