using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// Proves the responsive layout: at a phone-sized viewport the desktop nav is replaced by the
// hamburger button, and tapping it reveals the mobile menu with the nav links.
[Collection("ui")]
public class UiResponsiveTests
{
    private readonly BrowserFixture _fx;
    public UiResponsiveTests(BrowserFixture fx) => _fx = fx;

    [Fact]
    public async Task MobileViewport_HamburgerTogglesMenu()
    {
        var page = await _fx.NewPageAsync(width: 390, height: 844);   // iPhone-ish
        await page.GotoAsync("/");

        var hamburger = page.Locator("#mobile-menu-btn");
        var mobileMenu = page.Locator("#mobile-menu");

        // At this width the hamburger is shown and the menu starts hidden.
        await Assertions.Expect(hamburger).ToBeVisibleAsync();
        await Assertions.Expect(mobileMenu).ToBeHiddenAsync();

        // Tapping the hamburger opens the menu and reveals the nav links.
        await hamburger.ClickAsync();
        await Assertions.Expect(mobileMenu).ToBeVisibleAsync();
        await Assertions.Expect(mobileMenu.GetByRole(AriaRole.Link, new() { Name = "Players" })).ToBeVisibleAsync();
    }
}
