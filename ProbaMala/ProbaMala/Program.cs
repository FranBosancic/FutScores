using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Repositories;
using ProbaMala.Services;
using Serilog;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog: write to the console (dev convenience) and a daily rolling file under
// logs/. Levels and overrides come from the "Serilog" section of appsettings.json so
// verbosity can be tuned without a rebuild. logs/ is git-ignored.
builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine("logs", "futscores-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        rollOnFileSizeLimit: true));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "FutScores API",
        Version     = "v1",
        Description = "REST API for leagues, clubs, players, matches, ratings and users."
    });

    // Let the Swagger UI send a Bearer token with every request.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Paste your JWT here (without the 'Bearer ' prefix — the UI adds it).\n\nGet one from POST /api/auth/token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Let AJAX/Dropzone requests send the anti-forgery token in a header rather than
// a form field, so the image endpoints can keep [ValidateAntiForgeryToken].
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

var supportedCultures = new[]
{
    new CultureInfo("hr"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("hr");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// ASP.NET Core Identity: local accounts + roles, backed by AppDbContext.
// RequireConfirmedAccount = false because we don't send confirmation emails.
builder.Services
    .AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// JWT Bearer + policy scheme that dispatches to JWT when a Bearer header is
// present, and to Identity cookies otherwise. This lets both the MVC pages
// (cookie) and API clients (Bearer token) use the same [Authorize] attributes.
const string SmartScheme = "SmartAuth";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme         = SmartScheme;
    options.DefaultChallengeScheme = SmartScheme;
    options.DefaultForbidScheme   = SmartScheme;
})
// displayName is intentionally null: a non-null display name would make this
// internal routing scheme show up as a bogus "Continue with…" button on the
// login page (SignInManager.GetExternalAuthenticationSchemesAsync lists every
// scheme that has a display name).
.AddPolicyScheme(SmartScheme, displayName: null, options =>
{
    options.ForwardDefaultSelector = ctx =>
    {
        var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();
        if (auth?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            return JwtBearerDefaults.AuthenticationScheme;
        return IdentityConstants.ApplicationScheme;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// 3rd-party login (Google). Wired up only when credentials are present
// (stored in user-secrets for development, never committed to source), so the
// app still starts and runs normally when they are not configured.
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    builder.Services
        .AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}

// We never actually send mail; the framework's no-op sender keeps the default
// Identity pages (password reset / resend confirmation) from throwing.
builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<ISearchService, SearchService>();

// AI-assisted data entry (natural-language form prefill). Runs with the feature
// disabled when no Ai:ApiKey is configured, so the app still starts without a key.
// INameResolver turns the AI's names into database ids (the authoritative half).
builder.Services.AddScoped<IAiDataEntryService, AiDataEntryService>();
builder.Services.AddScoped<INameResolver, NameResolver>();

builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// One concise "GET /ratings responded 200 in 14ms" log line per request, instead of
// the framework's noisy default. Sits before the rest of the pipeline so it times the
// whole request.
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FutScores API v1");
    options.RoutePrefix = "swagger";   // UI at /swagger
});

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Relacijski provideri dobivaju shemu kroz migracije. In-memory provider koji
    // koriste integracijski testovi nije relacijski i bacio bi iznimku na Migrate(),
    // pa ga ondje preskačemo.
    if (dbContext.Database.IsRelational())
        dbContext.Database.Migrate();

    // Roles (Admin/User) + the configured default admin account.
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// Izloženo kako bi projekt s integracijskim testovima mogao podići stvarnu
// aplikaciju kroz WebApplicationFactory<Program>. Inače bi top-level statements
// kompajlirali ovaj entry-point razred kao internal.
public partial class Program { }
