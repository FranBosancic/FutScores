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

        // Optional link to the login (AppUser) that owns this profile. It is the
        // bridge that lets "edit your own rating" work: a signed-in user's ratings
        // are authored by the profile whose AppUserId matches their account.
        // Null for seeded or admin-created authors that were never tied to a login.
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}