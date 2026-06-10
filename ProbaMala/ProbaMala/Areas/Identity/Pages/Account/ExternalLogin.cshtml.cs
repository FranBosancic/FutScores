using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProbaMala.Data;
using ProbaMala.Models.Entities;

namespace ProbaMala.Areas.Identity.Pages.Account
{
    // Handles the Google (or any external provider) sign-in flow. This page
    // replaces the default Identity UI one because our AppUser carries required
    // OIB/JMBG fields, so a first-time external user must supply them before the
    // local account can be created.
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        // Display name of the provider the user just authenticated with (e.g. "Google").
        public string? ProviderDisplayName { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = null!;

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
        }

        // Reaching this page directly makes no sense — bounce back to Login.
        public IActionResult OnGet() => RedirectToPage("./Login");

        // Triggered by the "Continue with Google" button: kick off the OAuth challenge.
        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        // Provider redirects back here after the user authenticates with them.
        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign the user in with this external login if they already linked one.
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                    info.Principal.Identity?.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
                return RedirectToPage("./Lockout");

            // First time for this external login: show the form to finish registration.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)!
                };
            }

            return Page();
        }

        // Posted from the confirmation form: create the local account + link the login.
        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (!ModelState.IsValid)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                OIB = Input.OIB,
                JMBG = Input.JMBG,
                // The email comes verified from the external provider.
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                    // Same as local registration: a fresh external account is a plain User.
                    await _userManager.AddToRoleAsync(user, IdentitySeeder.UserRole);

                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                    return LocalRedirect(returnUrl);
                }
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
