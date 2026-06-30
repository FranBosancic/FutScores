using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.Entities;
using ProbaMala.Services;
using System.ComponentModel.DataAnnotations;

namespace ProbaMala.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtTokenService      _tokenService;
        private readonly IConfiguration       _config;
        private readonly ILogger<AuthApiController> _logger;

        public AuthApiController(
            UserManager<AppUser>  userManager,
            SignInManager<AppUser> signInManager,
            IJwtTokenService      tokenService,
            IConfiguration        config,
            ILogger<AuthApiController> logger)
        {
            _userManager  = userManager;
            _signInManager = signInManager;
            _tokenService  = tokenService;
            _config        = config;
            _logger        = logger;
        }

        // POST /api/auth/token
        // Returns a signed JWT on valid credentials. Use the token in subsequent
        // requests as:  Authorization: Bearer <token>
        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("JWT token request failed: no account for {Email}.", model.Email);
                return Unauthorized(new { error = "Invalid credentials." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("JWT token request failed: bad password for {Email}.", model.Email);
                return Unauthorized(new { error = "Invalid credentials." });
            }

            var roles     = await _userManager.GetRolesAsync(user);
            var token     = _tokenService.GenerateToken(user, roles);
            var expiryMin = Convert.ToInt32(_config["Jwt:ExpiryMinutes"] ?? "60");

            _logger.LogInformation(
                "JWT issued to {Email} (roles: {Roles}).",
                user.Email, roles.Count == 0 ? "none" : string.Join(", ", roles));

            return Ok(new
            {
                token,
                tokenType = "Bearer",
                expiresIn = expiryMin * 60   // seconds
            });
        }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
