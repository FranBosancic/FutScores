using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProbaMala.Models.Entities
{
    // Identifies which kind of entity an image belongs to. Used by the repository
    // and controllers to route an upload/list/delete to the right owner.
    public enum ImageOwnerType
    {
        Club,
        Player
    }

    // A single uploaded image. One image belongs to exactly one owner — either a
    // Club or a Player — so both foreign keys are nullable and only one is ever set.
    // The physical file lives under wwwroot/uploads/...; only metadata + the web
    // path are stored here.
    public class Image
    {
        [Key]
        public int Id { get; set; }

        // Original file name as uploaded by the user (for display/download).
        [Required]
        [MaxLength(260)]
        public string FileName { get; set; } = null!;

        // Web-accessible path, e.g. "/uploads/clubs/5/{guid}.png".
        [Required]
        [MaxLength(400)]
        public string FilePath { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        // Marks the "banner" (club) / "headshot" (player) — the one image shown
        // when a single representative image is needed. At most one per owner.
        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(Club))]
        public int? ClubId { get; set; }
        public virtual Club? Club { get; set; }

        [ForeignKey(nameof(Player))]
        public int? PlayerId { get; set; }
        public virtual Player? Player { get; set; }
    }
}
