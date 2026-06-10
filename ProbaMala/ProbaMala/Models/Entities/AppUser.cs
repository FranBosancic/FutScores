using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProbaMala.Models.Entities
{
    // Authentication account (ASP.NET Core Identity). This is separate from the
    // domain `User` entity — that one tracks the authors of ratings and has an
    // int key, whereas this one carries the login/credentials and a string key.
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OIB may only contain digits.")]
        public string OIB { get; set; } = null!;

        [Required]
        [StringLength(13, MinimumLength = 13)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "JMBG may only contain digits.")]
        public string JMBG { get; set; } = null!;
    }
}
