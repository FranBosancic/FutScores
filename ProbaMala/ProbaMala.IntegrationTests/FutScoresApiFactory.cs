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
    // pokrenutom SQL serveru ni o razvojnim podacima.
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
    }
}
