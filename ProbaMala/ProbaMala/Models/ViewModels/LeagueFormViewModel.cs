using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class LeagueFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "League name")]
        [Required(ErrorMessage = "Enter the league name.")]
        [StringLength(120, ErrorMessage = "League name can contain up to 120 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}