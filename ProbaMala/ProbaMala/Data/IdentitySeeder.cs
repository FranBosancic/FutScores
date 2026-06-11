using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Models.Entities;

namespace ProbaMala.Data
{
    // Ensures the Admin/User roles exist and that a default admin account is
    // present. Credentials come from configuration (the "SeedAdmin" section)
    // so nothing sensitive is hard-coded in source.
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            foreach (var role in new[] { AdminRole, UserRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var email = config["SeedAdmin:Email"];
            var password = config["SeedAdmin:Password"];

            // No admin credentials configured -> skip account seeding (roles still created).
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    OIB = config["SeedAdmin:OIB"] ?? "00000000000",
                    JMBG = config["SeedAdmin:JMBG"] ?? "0000000000000"
                };

                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed admin user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
                await userManager.AddToRoleAsync(admin, AdminRole);

            // Give the seeded admin a rating-author profile so they can post ratings
            // under a proper name (like any registered user). Skipped under the "Testing"
            // environment so the integration tests' in-memory DB stays free of domain users.
            var environment = services.GetRequiredService<IHostEnvironment>();
            if (!environment.IsEnvironment("Testing"))
            {
                var dbContext = services.GetRequiredService<AppDbContext>();
                if (!await dbContext.Users.AnyAsync(u => u.AppUserId == admin.Id))
                {
                    dbContext.Users.Add(new User
                    {
                        FirstName = "Site",
                        LastName = "Admin",
                        Email = admin.Email!,
                        AppUserId = admin.Id
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
