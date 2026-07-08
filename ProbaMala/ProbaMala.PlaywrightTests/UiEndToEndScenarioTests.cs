using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace ProbaMala.PlaywrightTests;

// The browser counterpart of EndToEndScenarioTests: ONE 10-step journey clicked through the
// real rendered UI as the admin. It builds a small graph (league → two clubs), verifies the
// relationship and the global search, edits, then deletes a club and the league (cascade) and
// proves they're gone. Every step drives actual DOM elements — forms, selects, nav, dropdown.
[Collection("ui")]
public class UiEndToEndScenarioTests
{
    private readonly BrowserFixture _fx;
    public UiEndToEndScenarioTests(BrowserFixture fx) => _fx = fx;

    [Fact]
    public async Task FullAdminJourney_TenSteps()
    {
        var tag      = Guid.NewGuid().ToString("N")[..6];
        var league   = $"UI League {tag}";
        var leagueEd = $"UI League {tag} EDIT";
        var club1    = $"UI Club A {tag}";
        var club2    = $"UI Club B {tag}";
        var userLast = $"User {tag}";
        var email    = $"ui{tag}@example.com";

        // ── Step 1: authenticate — the login form is filled and submitted. ──
        var page = await _fx.NewAdminPageAsync();
        await Assertions.Expect(page.GetByRole(AriaRole.Button, new() { Name = "Log out" }).First)
            .ToBeVisibleAsync();

        var userUrl = "";
        try
        {
            // ── Step 2: create a league via its form. ──
            await page.GotoAsync("/leagues");
            await page.GetByRole(AriaRole.Link, new() { Name = "Add league" }).ClickAsync();
            await page.FillAsync("input[name='Name']", league);
            await page.GetByRole(AriaRole.Button, new() { Name = "Save league" }).ClickAsync();
            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = league })).ToBeVisibleAsync();

            // ── Step 3: create the first club in that league. ──
            await CreateClubAsync(page, club1, league);
            await Assertions.Expect(page.GetByText(club1).First).ToBeVisibleAsync();

            // ── Step 4: create a second club in the same league; keep its URL for step 9. ──
            await CreateClubAsync(page, club2, league);
            await Assertions.Expect(page.GetByText(club2).First).ToBeVisibleAsync();
            var club2Url = page.Url;

            // ── Step 5: the league details now lists BOTH clubs (parent → children). ──
            await OpenLeagueAsync(page, league);
            await Assertions.Expect(page.GetByText(club1).First).ToBeVisibleAsync();
            await Assertions.Expect(page.GetByText(club2).First).ToBeVisibleAsync();

            // ── Step 6: create a rating author (user). ──
            await page.GotoAsync("/users");
            await page.GetByRole(AriaRole.Link, new() { Name = "Add user" }).ClickAsync();
            await page.FillAsync("input[name='FirstName']", "UI");
            await page.FillAsync("input[name='LastName']", userLast);
            await page.FillAsync("input[name='Email']", email);
            await page.GetByRole(AriaRole.Button, new() { Name = "Save user" }).ClickAsync();
            await Assertions.Expect(page.GetByText(userLast).First).ToBeVisibleAsync();
            userUrl = page.Url;

            // ── Step 7: find the first club through the global search box and open it. ──
            var box = page.Locator("[data-global-search]").First;
            await box.Locator("[data-global-search-input]").FillAsync(club1);
            await box.GetByRole(AriaRole.Link, new() { Name = club1 }).First.ClickAsync();
            await Assertions.Expect(page.GetByText(club1).First).ToBeVisibleAsync();
            Assert.Contains("club", page.Url, StringComparison.OrdinalIgnoreCase);

            // ── Step 8: rename the league (Edit) and see the new name on its details page. ──
            await OpenLeagueAsync(page, league);
            await page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
            await page.FillAsync("input[name='Name']", leagueEd);
            await page.GetByRole(AriaRole.Button, new() { Name = "Save changes" }).ClickAsync();
            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { Name = leagueEd })).ToBeVisibleAsync();

            // ── Step 9: delete the second club; the league drops back to a single club. ──
            await page.GotoAsync(club2Url);
            await page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Delete club" }).ClickAsync();
            await OpenLeagueAsync(page, leagueEd);
            await Assertions.Expect(page.GetByText(club1).First).ToBeVisibleAsync();
            await Assertions.Expect(page.GetByText(club2)).ToHaveCountAsync(0);

            // ── Step 10: delete the league (cascades the last club); it's gone from the list. ──
            await page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { Name = "Delete league" }).ClickAsync();
            await Assertions.Expect(page).ToHaveURLAsync(new Regex("/leagues", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.GetByText(leagueEd)).ToHaveCountAsync(0);
        }
        finally
        {
            // The user isn't cascade-deleted with the league — best-effort clean it up. The
            // create redirects to the details page (which may use the Croatian /korisnici alias),
            // so we just reopen whatever URL we landed on and delete from there.
            if (Regex.IsMatch(userUrl, @"\d+$"))
            {
                try
                {
                    await page.GotoAsync(userUrl);
                    await page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
                    await page.GetByRole(AriaRole.Button, new() { Name = "Delete user" }).ClickAsync();
                }
                catch { /* best effort */ }
            }
        }
    }

    // Creates a club through the /clubs form: name + league select. FoundedDate is pre-filled
    // with today by the form; we set it explicitly through flatpickr to be robust.
    private static async Task CreateClubAsync(IPage page, string clubName, string leagueLabel)
    {
        await page.GotoAsync("/clubs");
        await page.GetByRole(AriaRole.Link, new() { Name = "Add club" }).ClickAsync();
        await page.FillAsync("input[name='Name']", clubName);
        await page.SelectOptionAsync("select[name='LeagueId']", new SelectOptionValue { Label = leagueLabel });
        await page.EvaluateAsync(@"() => {
            const el = document.querySelector(""input[name='FoundedDate']"");
            if (el && el._flatpickr) el._flatpickr.setDate(new Date(2000, 0, 1), true);
        }");
        await page.GetByRole(AriaRole.Button, new() { Name = "Save club" }).ClickAsync();
    }

    // Opens a league's details page from the leagues list by its (unique) name. Scoped to the
    // list panel so it doesn't clash with the same league now shown in the nav bar.
    private static async Task OpenLeagueAsync(IPage page, string leagueName)
    {
        await page.GotoAsync("/leagues");
        await page.Locator("#league-filter-results")
            .GetByRole(AriaRole.Link, new() { Name = leagueName }).First.ClickAsync();
    }
}
