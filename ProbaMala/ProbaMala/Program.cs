using ProbaMala.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register mock repositories
builder.Services.AddSingleton<LeagueMockRepository>();
builder.Services.AddSingleton<ClubMockRepository>();
builder.Services.AddSingleton<PlayerMockRepository>();
builder.Services.AddSingleton<MatchMockRepository>();
builder.Services.AddSingleton<UserMockRepository>();
builder.Services.AddSingleton<RatingMockRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
