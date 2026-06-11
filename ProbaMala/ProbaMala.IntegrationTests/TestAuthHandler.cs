using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ProbaMala.IntegrationTests
{
    // Testni handler koji čita zaglavlje X-Test-Auth i bez prave baze korisnika
    // postavlja odgovarajuće claimove:
    //   "admin"       → Admin (i User) rola, NameIdentifier = "admin-test-id"
    //   "user:<id>"   → User rola, NameIdentifier = dani AppUserId
    //   (bez zaglavlja) → nije autentificiran (NoResult → 401)
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-Auth", out var authHeader))
                return Task.FromResult(AuthenticateResult.NoResult());

            var value = authHeader.ToString();
            List<Claim> claims;

            if (value == "admin")
            {
                claims = new List<Claim>
                {
                    new(ClaimTypes.Name, "admin@test.com"),
                    new(ClaimTypes.NameIdentifier, "admin-test-id"),
                    new(ClaimTypes.Role, "Admin"),
                    new(ClaimTypes.Role, "User")
                };
            }
            else if (value.StartsWith("user:"))
            {
                var appUserId = value["user:".Length..];
                claims = new List<Claim>
                {
                    new(ClaimTypes.Name, $"{appUserId}@test.com"),
                    new(ClaimTypes.NameIdentifier, appUserId),
                    new(ClaimTypes.Role, "User")
                };
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Nepoznata vrijednost zaglavlja."));
            }

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
