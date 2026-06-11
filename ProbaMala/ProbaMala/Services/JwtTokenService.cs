using Microsoft.IdentityModel.Tokens;
using ProbaMala.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProbaMala.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(AppUser user, IList<string> roles);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config) => _config = config;

        public string GenerateToken(AppUser user, IList<string> roles)
        {
            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"] ?? "60"));

            var claims = new List<Claim>
            {
                // NameIdentifier = AppUser.Id — used by the rating owner check
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.Email!),
                new(ClaimTypes.Email, user.Email!)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer:             _config["Jwt:Issuer"],
                audience:           _config["Jwt:Audience"],
                claims:             claims,
                expires:            expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
