using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Shared browser for the UI (click-through) tests. Launches ONE headless Chromium, reused
// by every UI test class via [Collection("ui")]. Each test opens its own browser context +
// page, so cookies/login never leak between tests. Runs against the RUNNING FutScores app
// (default http://localhost:5009, override FUTSCORES_URL) — same target as the API tests,
// but driven through the real rendered UI instead of raw HTTP.
public class BrowserFixture : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    public IBrowser Browser { get; private set; } = null!;

    public string BaseUrl { get; } =
        Environment.GetEnvironmentVariable("FUTSCORES_URL") ?? "http://localhost:5009";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null) await Browser.CloseAsync();
        _playwright?.Dispose();
    }

    // A fresh anonymous page. Default viewport is wide enough (>= xl / 1280px) that the full
    // desktop navigation is shown rather than the hamburger.
    public async Task<IPage> NewPageAsync(int width = 1366, int height = 900)
    {
        var context = await Browser.NewContextAsync(new()
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true,
            ViewportSize = new() { Width = width, Height = height }
        });
        return await context.NewPageAsync();
    }

    // A page already signed in as the seeded admin.
    public async Task<IPage> NewAdminPageAsync()
    {
        var page = await NewPageAsync();
        await LoginAsync(page);
        return page;
    }

    // Fills the Identity login form and waits until the "Log out" control appears, proving
    // the session is established before the test continues.
    public static async Task LoginAsync(IPage page)
    {
        await page.GotoAsync("/Identity/Account/Login");
        await page.FillAsync("input[name='Input.Email']", "admin@futscores.local");
        await page.FillAsync("input[name='Input.Password']", "Admin123!");
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Button, new() { Name = "Log out" }).First)
            .ToBeVisibleAsync();
    }
}

[CollectionDefinition("ui")]
public class UiCollection : ICollectionFixture<BrowserFixture> { }
