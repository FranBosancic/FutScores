using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(180)]
        public string Email { get; set; } = null!;

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}