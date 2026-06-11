using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProbaMala.Data;
using ProbaMala.Models.Entities;

namespace ProbaMala.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _db;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext db,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

            [Required]
            [StringLength(120)]
            [Display(Name = "First name")]
            public string FirstName { get; set; } = null!;

            [Required]
            [StringLength(120)]
            [Display(Name = "Last name")]
            public string LastName { get; set; } = null!;

            [Required]
            [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB must be exactly 11 digits.")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "OIB may only contain digits.")]
            [Display(Name = "OIB")]
            public string OIB { get; set; } = null!;

            [Required]
            [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG must be exactly 13 digits.")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "JMBG may only contain digits.")]
            [Display(Name = "JMBG")]
            public string JMBG { get; set; } = null!;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = null!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = null!;
        }

        public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                OIB = Input.OIB,
                JMBG = Input.JMBG
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Every self-registration is a plain User. Admins are seeded/assigned separately.
                await _userManager.AddToRoleAsync(user, IdentitySeeder.UserRole);

                // Create the rating-author profile tied to this login, so the user can
                // post — and later edit/delete — their own ratings.
                _db.Users.Add(new ProbaMala.Models.Entities.User
                {
                    FirstName = Input.FirstName.Trim(),
                    LastName = Input.LastName.Trim(),
                    Email = Input.Email,
                    AppUserId = user.Id
                });
                await _db.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
