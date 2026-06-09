using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class UserRequest
    {
        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(180)]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
