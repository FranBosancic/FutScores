# FutScores Playwright API tests

End-to-end **API** tests written with [Playwright for .NET](https://playwright.dev/dotnet/)
(`IAPIRequestContext` — no browser). They exercise the real HTTP stack (routing, model
binding, validation, JWT auth, EF, JSON) against a **running** FutScores app.

- **Per-endpoint coverage** — one test class per entity (Leagues, Clubs, Players, Matches,
  Ratings, Users) covering `GET all`, `GET by id` (200 + 404), `POST` (201 + 400
  validation), `PUT` (200 + 404) and `DELETE` (204 + 404), plus `AuthApiTests` for the
  token endpoint and a negative-auth check.
- **`EndToEndScenarioTests`** — the bonus **10-step scenario**: authenticate → create a
  league → two clubs → a player → a match → a user → rate the player → read → update →
  delete → assert 404. Each step chains ids from the previous ones.

The tests run against the dev database and **clean up everything they create** (unique
`PW`/`E2E` tags + deletes in reverse dependency order).

## Run

1. Start Postgres + the app:
   ```
   docker compose -f ProbaMala/docker-compose.yml up -d postgres
   dotnet run --project ProbaMala/ProbaMala --launch-profile http
   ```
2. In another terminal:
   ```
   dotnet test ProbaMala/ProbaMala.PlaywrightTests
   ```

The base URL defaults to `http://localhost:5009`; override with the `FUTSCORES_URL`
environment variable. No `playwright install` is needed — API testing doesn't use a
browser, only the bundled driver.
