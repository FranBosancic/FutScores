using ProbaMala.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.DTOs
{
    public class PlayerDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = null!;
        public string Nationality { get; set; } = null!;
        public ClubSummaryDTO Club { get; set; } = null!;
    }

    public class PlayerRequest
    {
        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string LastName { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        [MaxLength(120)]
        public string Nationality { get; set; } = null!;

        [Required]
        public int? ClubId { get; set; }
    }
}
