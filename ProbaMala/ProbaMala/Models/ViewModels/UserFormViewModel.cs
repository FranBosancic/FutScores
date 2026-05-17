using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Models.ViewModels
{
    public class UserFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "Enter the user's first name.")]
        [StringLength(120, ErrorMessage = "First name can contain up to 120 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Enter the user's last name.")]
        [StringLength(120, ErrorMessage = "Last name can contain up to 120 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter the user's email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(180, ErrorMessage = "Email can contain up to 180 characters.")]
        public string Email { get; set; } = string.Empty;
    }
}