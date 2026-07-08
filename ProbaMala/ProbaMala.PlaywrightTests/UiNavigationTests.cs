using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Click-through smoke tests for the main navigation, driven through the real UI.
[Collection("ui")]
public class UiNavigationTests
{
    private readonly BrowserFixture _fx;
    public UiNavigationTests(BrowserFixture fx) => _fx = fx;

    [Fact]
    public async Task Dashboard_LoadsWithStatCards()
    {
        var page = await _fx.NewPageAsync();
        await page.GotoAsync("/");

        await Assertions.Expect(page).ToHaveTitleAsync(new Regex("Dashboard"));

        // Scope to <main> so the stat labels don't collide with the nav's hidden "Clubs" links.
        var main = page.Locator("main");
        await Assertions.Expect(main.GetByText("CLUBS").First).ToBeVisibleAsync();
        await Assertions.Expect(main.GetByText("PLAYERS").First).ToBeVisibleAsync();
        await Assertions.Expect(main.GetByText("MATCHES").First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Navbar_Players_NavigatesToPlayersList()
    {
        var page = await _fx.NewPageAsync();
        await page.GotoAsync("/");

        // Click the "Players" tab in the desktop nav (scoped so it can't hit the mobile menu).
        await page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Players" }).ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(new Regex("/players", RegexOptions.IgnoreCase));
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex("Players"));
    }

    [Fact]
    public async Task Navbar_Ratings_NavigatesToRatingsList()
    {
        var page = await _fx.NewPageAsync();
        await page.GotoAsync("/");

        await page.Locator("nav").GetByRole(AriaRole.Link, new() { Name = "Ratings" }).ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(new Regex("/ratings", RegexOptions.IgnoreCase));
    }
}
