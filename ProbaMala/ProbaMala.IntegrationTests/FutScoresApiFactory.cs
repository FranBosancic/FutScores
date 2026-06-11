using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProbaMala.Data;

namespace ProbaMala.IntegrationTests
{
    // Podiže stvarnu aplikaciju — routing, model binding, validacija, controlleri,
    // EF i JSON serijalizacija sve se izvršava stvarno — ali zamjenjuje AppDbContext
    // koji koristi PostgreSQL s EF in-memory bazom, tako da testovi nikad ne ovise o
    // pokrenutom SQL serveru ni o razvojnim podacima. Autentikacijska shema je
    // zamijenjena s TestAuthHandler koji prima claimove iz zaglavlja X-Test-Auth.
    public class FutScoresApiFactory : WebApplicationFactory<Program>
    {
        // Jedinstveno ime baze po instanci factoryja. Budući da svaki test radi svoj
        // factory, time svaki test dobije potpuno svježu i izoliranu in-memory bazu.
        private readonly string _databaseName = $"FutScoresTests-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Ukloni registraciju PostgreSQL DbContexta iz aplikacije...
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                // ...i zamijeni je in-memory bazom za ovaj test.
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));

                // Registriraj testni auth handler koji čita zaglavlje X-Test-Auth.
                services.AddAuthentication()
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName, null);

                // PostConfigure se izvršava ZADNJI (nakon svih Configure poziva, uključujući
                // Identity-jeve), pa sigurno pregazuje podrazumijevane scheme s testnim.
                services.PostConfigure<AuthenticationOptions>(o =>
                {
                    o.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    o.DefaultChallengeScheme    = TestAuthHandler.SchemeName;
                    o.DefaultForbidScheme       = TestAuthHandler.SchemeName;
                });
            });
        }

        // Izvršava akciju nad svježim AppDbContext scopeom koji koristi istu in-memory
        // bazu kao i aplikacija — korisno za seedanje testnih podataka ili za provjeru
        // spremljenog stanja nakon API poziva.
        public async Task WithDbContextAsync(Func<AppDbContext, Task> action)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await action(db);
        }

        // Klijent koji šalje X-Test-Auth: admin → Admin (i User) rola.
        public HttpClient CreateAdminClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-Auth", "admin");
            return client;
        }

        // Klijent koji šalje X-Test-Auth: user:<appUserId> → User rola s danim identitetom.
        public HttpClient CreateUserClient(string appUserId)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-Auth", $"user:{appUserId}");
            return client;
        }
    }
}
