using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class RatingFormViewModel
    {
        public int Id { get; set; }

        // ── Cascade: League → Home team → Away team → Match → Player ──
        // Each step narrows the next, so an invalid combination can't be built.
        // LeagueId / HomeTeamId / AwayTeamId are not stored on the Rating entity
        // directly — they're derived from the chosen Match — but they drive the
        // form's progressive selection (and let us re-render it after a postback).

        [Display(Name = "League")]
        [Required(ErrorMessage = "Select a league.")]
        public int? LeagueId { get; set; }

        [Display(Name = "Home team")]
        [Required(ErrorMessage = "Select the home team.")]
        public int? HomeTeamId { get; set; }

        [Display(Name = "Away team")]
        [Required(ErrorMessage = "Select the away team.")]
        public int? AwayTeamId { get; set; }

        [Display(Name = "Match")]
        [Required(ErrorMessage = "Select a match.")]
        public int? MatchId { get; set; }

        [Display(Name = "Player")]
        [Required(ErrorMessage = "Select a player.")]
        public int? PlayerId { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "Select the user who submitted the rating.")]
        public int? UserId { get; set; }

        [Display(Name = "Score")]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
        public int Score { get; set; } = 1;

        [Display(Name = "Comment")]
        [StringLength(400, ErrorMessage = "Comment can contain up to 400 characters.")]
        public string? Comment { get; set; }

        // Display-only labels used by the Edit page intro sentence.
        public string? PlayerLabel { get; set; }
        public string? MatchLabel { get; set; }
        public string? UserLabel { get; set; }

        // Server-rendered option lists. Dependent lists are only populated when
        // their parent value is known (on Edit, or when re-displaying an invalid
        // post); otherwise the matching <select> starts empty and disabled and is
        // filled in by the cascade script.
        public IEnumerable<SelectListItem> LeagueOptions { get; set; } = [];
        public IEnumerable<SelectListItem> HomeTeamOptions { get; set; } = [];
        public IEnumerable<SelectListItem> AwayTeamOptions { get; set; } = [];
        public IEnumerable<SelectListItem> MatchOptions { get; set; } = [];
        public IEnumerable<SelectListItem> PlayerOptions { get; set; } = [];
        public IEnumerable<SelectListItem> UserOptions { get; set; } = [];
    }
}
