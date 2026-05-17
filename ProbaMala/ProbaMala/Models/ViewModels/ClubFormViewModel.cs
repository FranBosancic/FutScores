using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class ClubFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Club name")]
        [Required(ErrorMessage = "Enter the club name.")]
        [StringLength(160, ErrorMessage = "Club name can contain up to 160 characters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Founded date")]
        [DataType(DataType.Date)]
        public DateTime FoundedDate { get; set; } = DateTime.Today;

        [Display(Name = "League")]
        [Required(ErrorMessage = "Select the league this club belongs to.")]
        public int? LeagueId { get; set; }

        public IEnumerable<SelectListItem> LeagueOptions { get; set; } = [];
    }
}