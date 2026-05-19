using Microsoft.AspNetCore.Mvc.Rendering;
using ProbaMala.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class PlayerFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "Enter the player's first name.")]
        [StringLength(120, ErrorMessage = "First name can contain up to 120 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Enter the player's last name.")]
        [StringLength(120, ErrorMessage = "Last name can contain up to 120 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Today;

        [Display(Name = "Position")]
        [Required(ErrorMessage = "Select the player's position.")]
        public Position? Position { get; set; }

        [Display(Name = "Club")]
        [Required(ErrorMessage = "Select the player's club.")]
        public int? ClubId { get; set; }

        [Display(Name = "Nationality")]
        [Required(ErrorMessage = "Enter the player's nationality.")]
        [StringLength(120, ErrorMessage = "Nationality can contain up to 120 characters.")]
        public string Nationality { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> ClubOptions { get; set; } = [];
        public IEnumerable<SelectListItem> PositionOptions { get; set; } = [];
    }
}