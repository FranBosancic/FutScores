using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class RatingFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Player")]
        [Required(ErrorMessage = "Select a player.")]
        public int? PlayerId { get; set; }

        [Display(Name = "Match")]
        [Required(ErrorMessage = "Select a match.")]
        public int? MatchId { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "Select the user who submitted the rating.")]
        public int? UserId { get; set; }

        [Display(Name = "Score")]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
        public int Score { get; set; } = 1;

        [Display(Name = "Comment")]
        [StringLength(400, ErrorMessage = "Comment can contain up to 400 characters.")]
        public string? Comment { get; set; }

        public string? PlayerLabel { get; set; }
        public string? MatchLabel { get; set; }
        public string? UserLabel { get; set; }

        public IEnumerable<SelectListItem> MatchOptions { get; set; } = [];
        public IEnumerable<SelectListItem> UserOptions { get; set; } = [];
    }
}