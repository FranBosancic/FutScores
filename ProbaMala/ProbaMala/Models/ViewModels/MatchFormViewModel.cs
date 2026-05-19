using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class MatchFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "League")]
        [Required(ErrorMessage = "Select the league.")]
        public int? LeagueId { get; set; }

        [Display(Name = "Home team")]
        [Required(ErrorMessage = "Select the home team.")]
        public int? HomeTeamId { get; set; }

        [Display(Name = "Away team")]
        [Required(ErrorMessage = "Select the away team.")]
        public int? AwayTeamId { get; set; }

        [Display(Name = "Kickoff")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Home goals")]
        [Range(0, 99, ErrorMessage = "Home goals must be between 0 and 99.")]
        public int HomeGoals { get; set; }

        [Display(Name = "Away goals")]
        [Range(0, 99, ErrorMessage = "Away goals must be between 0 and 99.")]
        public int AwayGoals { get; set; }

        public IEnumerable<SelectListItem> LeagueOptions { get; set; } = [];
        public IEnumerable<SelectListItem> ClubOptions { get; set; } = [];
    }
}