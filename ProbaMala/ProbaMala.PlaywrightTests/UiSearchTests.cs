using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Drives the global search box through the UI: type a query, see the live dropdown, click a
// result, land on the right details page. Relies on the seeded "FC Barcelona" club.
[Collection("ui")]
public class UiSearchTests
{
    private readonly BrowserFixture _fx;
    public UiSearchTests(BrowserFixture fx) => _fx = fx;

    [Fact]
    public async Task GlobalSearch_ShowsLiveResults_AndNavigatesToDetails()
    {
        // Signed in, because entity details pages require authentication — an anonymous click
        // on a result would bounce to the login page instead of the club.
        var page = await _fx.NewAdminPageAsync();
        await page.GotoAsync("/");

        // The desktop search box is the first [data-global-search] in the header.
        var box = page.Locator("[data-global-search]").First;
        await box.Locator("[data-global-search-input]").FillAsync("barcelona");

        // The debounced fetch drops a rendered partial into the dropdown; a Barcelona link appears.
        var result = box.GetByRole(AriaRole.Link, new() { Name = "Barcelona" }).First;
        await Assertions.Expect(result).ToBeVisibleAsync();

        await result.ClickAsync();

        // We navigated to a club details page that shows the club name.
        await Assertions.Expect(page.GetByText(new Regex("Barcelona", RegexOptions.IgnoreCase)).First)
            .ToBeVisibleAsync();
        Assert.Contains("club", page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GlobalSearch_EmptyFocus_ShowsPageMenu()
    {
        var page = await _fx.NewPageAsync();
        await page.GotoAsync("/");

        var box = page.Locator("[data-global-search]").First;
        // Focusing the empty box shows the full "jump to page" menu.
        await box.Locator("[data-global-search-input]").ClickAsync();

        await Assertions.Expect(box.GetByRole(AriaRole.Link, new() { Name = "Players" })).ToBeVisibleAsync();
        await Assertions.Expect(box.GetByRole(AriaRole.Link, new() { Name = "Matches" })).ToBeVisibleAsync();
    }
}
