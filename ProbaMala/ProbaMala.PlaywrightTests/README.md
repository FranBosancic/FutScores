# FutScores Playwright tests

Tests written with [Playwright for .NET](https://playwright.dev/dotnet/), in **two flavours**,
both against a **running** FutScores app:

## API tests (`IAPIRequestContext`, no browser) — 42 tests

Exercise the real HTTP stack (routing, model binding, validation, JWT auth, EF, JSON).

- **Per-endpoint coverage** — one test class per entity (Leagues, Clubs, Players, Matches,
  Ratings, Users) covering `GET all`, `GET by id` (200 + 404), `POST` (201 + 400
  validation), `PUT` (200 + 404) and `DELETE` (204 + 404), plus `AuthApiTests` for the
  token endpoint and a negative-auth check, `SearchApiTests`, and `FilterApiTests`.
- **`EndToEndScenarioTests`** — the bonus **10-step scenario**: authenticate → create a
  league → two clubs → a player → a match → a user → rate the player → read → update →
  delete → assert 404. Each step chains ids from the previous ones.

## Browser (UI) tests — 7 tests

Drive the real rendered UI in a headless Chromium via `BrowserFixture` (`[Collection("ui")]`):

- **`UiNavigationTests`** — dashboard loads with stat cards; nav tabs route to the right lists.
- **`UiSearchTests`** — the global search box shows live results and navigates to details;
  focusing the empty box shows the page menu.
- **`UiResponsiveTests`** — at a phone viewport the hamburger replaces the nav and toggles
  the mobile menu.
- **`UiEndToEndScenarioTests`** — the browser counterpart of the API 10-step scenario: one
  **10-step journey** clicked through the UI as admin — log in → create a league → two clubs
  → verify both appear under the league → create a user → find a club via global search →
  rename the league (Edit) → delete a club → delete the league (cascade) → confirm it's gone.

Both flavours run against the dev database and **clean up everything they create** (unique
`PW`/`E2E`/`UI` tags + deletes in reverse dependency order).

## Run

1. Start Postgres + the app:
   ```
   docker compose -f ProbaMala/docker-compose.yml up -d postgres
   dotnet run --project ProbaMala/ProbaMala --launch-profile http
   ```
2. One-time: install the Chromium browser for the UI tests (after a build, so the script exists):
   ```
   dotnet build ProbaMala/ProbaMala.PlaywrightTests
   pwsh ProbaMala/ProbaMala.PlaywrightTests/bin/Debug/net8.0/playwright.ps1 install chromium
   ```
   (The API tests don't need this — only the browser tests do.)
3. In another terminal:
   ```
   dotnet test ProbaMala/ProbaMala.PlaywrightTests
   ```

The base URL defaults to `http://localhost:5009`; override with the `FUTSCORES_URL`
environment variable. Run only one flavour with a filter, e.g.
`dotnet test ... --filter "FullyQualifiedName~Ui"` (browser) or `~Api` (HTTP).
