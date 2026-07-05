# FutScores — Final Project Implementation Plan

This document maps every criterion in [`project_requirements.txt`](project_requirements.txt)
to the parts of the codebase responsible for it: what already exists, and which
folders/classes we add or change to satisfy each step. It is the working reference
for the remaining project work (70 points total).

> The app: **FutScores** — a football platform where signed-in users rate player
> performances after matches. ASP.NET Core 8 MVC + REST API, PostgreSQL (EF Core /
> Npgsql), ASP.NET Core Identity. The actual project is `ProbaMala/ProbaMala`.

---

## 1. Architecture at a glance

All paths are relative to `ProbaMala/ProbaMala/` unless noted.

| Layer | Folder | Responsibility |
| --- | --- | --- |
| **Entities** (DB model) | `Models/Entities/` | EF Core entities: `League`, `Club`, `Player`, `Match`, `Rating`, `User`, `Image`, `AppUser`, `Position` enum |
| **DTOs** (API shapes) | `Models/DTOs/` | What the REST API returns/accepts — never raw entities |
| **ViewModels** (MVC shapes) | `Models/ViewModels/` | What Razor views bind to (form + details models) |
| **Data access** | `Repositories/` | One repo per aggregate (`*Repository.cs` + `I*Repository`), all EF queries live here |
| **DB context + seed** | `Data/` | `AppDbContext` (`IdentityDbContext<AppUser>`), `IdentitySeeder` |
| **MVC controllers** | `Controllers/` | Page controllers returning Razor views; custom Croatian routes via `[Route]` |
| **API controllers** | `Controllers/Api/` | `[ApiController]` REST endpoints `api/<entity>`, `AuthApiController` issues JWT |
| **Services** | `Services/` | Cross-cutting services (`JwtTokenService`) |
| **Views** | `Views/<Entity>/` | `Index`, `Details`, `Create`, `Edit`, `Delete`, plus `_*Form` / `_*List` partials |
| **Shared UI** | `Views/Shared/` | `_Layout` (Tailwind), `_LoginPartial`, `_DateTimePicker`, `_ImageList` |
| **Auth pages** | `Areas/Identity/Pages/Account/` | Register / Login / ExternalLogin (Google) |
| **Migrations** | `Migrations/` | EF Core migrations (4 so far) |
| **Static assets** | `wwwroot/` | `js/site.js`, `css/site.css`, Tailwind output, `lib/`, `uploads/` |
| **Config** | `Program.cs`, `appsettings.json` | DI registration, auth, localization, Swagger, connection string |
| **Integration tests** | `ProbaMala/ProbaMala.IntegrationTests/` | xUnit + `WebApplicationFactory` + EF InMemory, 114 tests |

**The dominant pattern** (follow it for any new feature): a request hits an MVC or
API **controller** → the controller calls a **repository interface** (DI-injected) →
the repository runs EF queries and maps entities to a **DTO/ViewModel** → the result
goes to a **view** (MVC) or is serialized (API). Controllers stay thin; all data
logic is in repositories.

### Domain model (relationships)

```
League 1───N Club 1───N Player
   │            │           │
   │            └──N Match (HomeTeam / AwayTeam, both → Club)   N───1 League
   │                  │
   │                  └──N Rating ──N───1 Player
   │                              ──N───1 User (domain rating author)
Club/Player 1───N Image
AppUser (Identity login) 1───0..1 User (domain profile, via User.AppUserId)
```

`User` (rating author, int key) is **separate** from `AppUser` (Identity login,
string key). A signed-in account is linked to its author profile via `User.AppUserId`.

---

## 2. Requirement-by-requirement map

Status legend: ✅ done · 🟡 partial · ❌ to build.

### 2.1 Deploy to cloud provider / VM — 3 pts ❌

**Goal:** the app running on Azure, Google Cloud, or a VM, reachable over the internet.

- **What exists:** `ProbaMala/docker-compose.yml` (Postgres + Adminer only — no app
  container yet). Connection string in `appsettings.json` (`ConnectionStrings:Postgres`).
- **To add:**
  - `ProbaMala/ProbaMala/Dockerfile` — multi-stage build (`sdk` → `aspnet` runtime)
    publishing the web app.
  - Extend `docker-compose.yml` (or a deploy compose) with an `app` service depending
    on `postgres`, env-var-driven connection string and secrets.
  - Host config: connection string + `Jwt:Key` + Google secrets via **environment
    variables** (not committed). `Program.cs` already reads them through `IConfiguration`.
  - `app.UseHttpsRedirection()` and the migration-on-startup block in `Program.cs`
    already make first-run setup automatic (`Database.Migrate()` runs for relational DBs).
- **Decision needed:** target host (Azure App Service / Azure Container Apps / a Linux
  VM with Docker / Google Cloud Run). Affects the deploy scripts only, not app code.

### 2.2 Playwright tests for all API endpoints — 2 pts (+3 extra) ❌

**Goal:** a Playwright scenario of at least 10 steps covering the API endpoints
(+3 bonus for extra coverage). Note: this is **separate** from the existing xUnit
integration tests — the lab asks specifically for Playwright.

- **What exists:** `ProbaMala/ProbaMala.IntegrationTests/` (xUnit, 114 tests) — good
  reference for endpoint shapes and auth, but not Playwright.
- **To add:**
  - New test project/folder, e.g. `ProbaMala/ProbaMala.E2ETests/` (Playwright for .NET,
    `Microsoft.Playwright` + NUnit/xUnit) **or** a standalone Node Playwright project.
  - A 10+ step scenario chaining real HTTP calls against a running instance:
    e.g. `POST /api/auth/token` → create league → create club → create player →
    create match → create rating → GET lists → PUT → DELETE → assert 404.
  - Auth: obtain a JWT from `AuthApiController` (`POST /api/auth/token`) and send it
    as a Bearer header for the Admin-only mutations.
- **Touches:** no app code — purely a new test project driving existing endpoints
  (`Controllers/Api/*`).

### 2.3 AI integration — AI-assisted data entry — 3 pts 🟢 (built for all entities; needs API key to run live)

**Goal:** let the user enter data through a natural-language prompt (e.g. "Salah was
outstanding in Liverpool's win over Man City — give him a 9"). Now on **all five**
Create pages: Rating, Player, Club, Match, User.

**Chosen design (with the user):** target the **Rating** entity; the AI **pre-fills the
existing Create form** and the human confirms (nothing is written by the AI). Provider:
**Claude** via the official `Anthropic` C# SDK, structured-output mode. Model is configured
in `Ai:Model` — currently **`claude-haiku-4-5`** (fast/cheap; swap without code changes).

- **Implemented (2026-07-02):**
  - `Models/DTOs/RatingAiIntent.cs` — the structured extraction shape (player + two club
    names + score + comment). Names, not ids.
  - `Services/IAiDataEntryService` + `AiDataEntryService` — calls Claude with a JSON
    schema (`OutputConfig.Format`), deserializes the reply to `RatingAiIntent`. Behind an
    interface; `IsConfigured` is false when no key, so the app runs and the AI box hides.
    Registered `AddScoped`.
  - `RatingRepository` resolution helpers — `FindClubIdByName`, `FindMatchIdBetween`
    (order-independent), `FindPlayerIdByNameInMatch`. **Our code owns name→id resolution
    and validation; the AI only does language understanding.**
  - `RatingController.AiFill(prompt)` (`POST /ratings/ai`, `[Authorize]`) — extract →
    resolve → reuse `BuildFormModel(matchId, playerId)` → re-render the Create form
    pre-filled, with a note saying how far resolution got. Falls back gracefully when a
    club/match/player can't be matched.
  - `Views/Rating/Create.cshtml` — an "✨ AI assist" prompt box (shown only when
    configured) posting to `AiFill`, plus a note banner and a model-error summary.
  - Config: `Ai` section in `appsettings.json` (`Model`, empty `ApiKey`); the real key
    goes in **user-secrets** (never committed). Package: `Anthropic` 12.35.0.
  - Build (incl. Razor) + 122 integration tests pass. The live call is unverified until
    the user adds an Anthropic API key (`dotnet user-secrets set "Ai:ApiKey" "…"`).
- **Extended (2026-07-03):**
  - Generalized the service to `ExtractRatingAsync` / `ExtractPlayerAsync` /
    `ExtractClubAsync` over one private `ExtractAsync<T>` (system prompt + JSON schema in,
    typed intent out). New DTOs `PlayerAiIntent`, `ClubAiIntent`; date parsing in
    `Services/AiParsing`.
  - Name→id resolution moved into a shared `Services/INameResolver` (`ResolveClub`,
    `ResolveLeagueId`, `ResolveMatchId`, `ResolvePlayerIdInMatch`) — the RatingRepository
    helpers were removed and RatingController now uses the resolver too. One place owns
    resolution for all AI entities.
  - `AiFill` added to **PlayerController** (resolves club) and **ClubController** (resolves
    league), Admin-only, same pre-fill-and-confirm flow.
  - Reusable `Views/Shared/_AiAssistBox.cshtml` partial (per-page placeholder via
    `ViewData["AiPlaceholder"]`); used on Rating/Player/Club Create pages.
  - **Loading overlay**: `site.js` shows a full-screen "FutScores AI is filling the form…"
    spinner + disables the button on any `[data-ai-form]` submit; replaced when the
    pre-filled page renders. Verified in-browser (box + placeholder + overlay behaviour).
  - **Match + User added (2026-07-03):** `MatchAiIntent` (resolves both clubs; league
    derived from the home club) and `UserAiIntent` (name + email, no FK). `ExtractMatchAsync`
    / `ExtractUserAsync`, `AiFill` on Match/UserController, AI box on both Create pages.
    All five entities verified rendering the box in-browser; build + 122 tests pass.
- **Remaining:** add the API key and smoke-test the live model. Feature is otherwise
  complete across all entities.

### 2.4 Global search — 2 pts ✅ (menus/pages + data)

**Goal:** one search box that searches across **menus/pages and data** (leagues,
clubs, players, matches, ratings, users), not just within one list.

- **Tier 1 — menus/pages ✅ (done 2026-06-30):**
  - `Models/ViewModels/SearchResultViewModel.cs` — generic result row (Title, Category,
    Url) reused by every tier.
  - `Services/ISearchService` + `SearchService` — a static catalogue of the app's
    pages, matched on title + hidden keywords; URLs resolved via `LinkGenerator` from
    the named routes. Empty query returns the whole catalogue (focus = "jump to page"
    menu). Registered `AddScoped` in `Program.cs`.
  - `Controllers/SearchController` — `GET /search` (Croatian alias `/pretraga`),
    `[AllowAnonymous]`, returns the `_SearchResults` partial.
  - `Views/Shared/_SearchResults.cshtml` — dropdown list partial.
  - `Views/Shared/_Layout.cshtml` — search box in the header (desktop/tablet) and in the
    mobile menu; wired in `wwwroot/js/site.js` (`[data-global-search]`, debounced fetch,
    focus-to-open, click-outside/Escape to close).
  - Verified in the browser preview (desktop + mobile) and via the endpoint (title +
    keyword matches); 122 integration tests still pass.
- **Tier 2 — data ✅ (done 2026-06-30):**
  - `Repositories/ISearchRepository` + `SearchRepository` — fans out across leagues,
    clubs, players, matches, ratings and users with minimal per-entity projections,
    caps each type (default 5), and maps every hit to `SearchResultViewModel`
    (Category = entity type, Url = its details page via `LinkGenerator`). Returns
    nothing for a blank query (never dumps the whole DB). Registered `AddScoped`.
  - `SearchService` now orchestrates: matching pages first, then the merged data hits.
  - `_SearchResults.cshtml` groups results by Category into section headers (Pages,
    League, Club, Player, Match, Rating, User).
  - **Why a dedicated repo over reusing the list repos:** search needs a lightweight
    projection + per-type cap, it's a cross-cutting concern kept in one place, and it
    avoids growing/risking the six tested list repositories. (Slight `Contains`
    duplication accepted — cohesion over strict DRY.)
  - Verified in the browser preview (e.g. "barcelona" → Club + Match groups; "salah" →
    Player + Rating) and via the endpoint; 122 integration tests still pass.

### 2.5 Logging mechanism (file or API) — 2 pts ✅

**Goal:** application logging to a file or a logging API.

- **Implemented with Serilog → rolling daily file:**
  - Package `Serilog.AspNetCore` 8.0.3 (net8-aligned) in `ProbaMala.csproj`.
  - `Program.cs`: `builder.Host.UseSerilog(...)` writes to console + a daily rolling
    file `logs/futscores-<date>.log` (14 files retained, size-capped); levels read from
    the `Serilog` section of `appsettings.json`. `app.UseSerilogRequestLogging()` adds
    one concise line per request.
  - `appsettings.json`: old `Logging` section replaced by a `Serilog` section
    (Default Information; AspNetCore + EF Core overridden to Warning).
  - `.gitignore`: `logs/` (ignored at any depth).
  - Meaningful domain log calls on **every create/update/delete across all
    controllers** — MVC (`League`, `Club`, `Player`, `Match`, `User`, `Rating`, plus
    club/player image upload+delete) and API (`League`, `Club`, `Player`, `Match`,
    `User`, `Rating`) — plus `AuthApiController` (JWT issued / failed login) and
    ownership-denied **warnings** on rating edit/delete. Reads are covered generically
    by `UseSerilogRequestLogging`.
  - Verified at runtime and via the test run (the 122 integration tests drive the API
    mutations, and the produced log file contains the matching create/update/delete +
    "forbidden (not owner)" entries). All 122 tests pass.

### 2.6 Responsive mobile/web UI — 2 pts 🟡 (mostly done)

- **What exists:** `Views/Shared/_Layout.cshtml` is fully Tailwind-based with a
  desktop nav (`xl:flex`) and a separate mobile hamburger menu (`xl:hidden`), responsive
  grids in dashboard/list/detail views. Localization (hr/en) and a custom flatpickr
  date picker (`_DateTimePicker`) are in place.
- **To verify/polish:** walk each page at mobile width (forms, tables, the rating
  cascade) and fix any overflow. Low effort; mostly already satisfied.

### 2.7 CRUD works without errors — 2 pts ✅

- Full CRUD exists for all entities on both the **MVC** side (`Controllers/*` + Views)
  and the **API** side (`Controllers/Api/*`), backed by repositories, covered by 114
  integration tests. Keep it green; re-run `dotnet test` after changes.

### 2.8 Expose MCP + access through an agentic IDE — 2 pts ❌

**Goal:** expose the app's data/operations as an **MCP server** so an agentic IDE can
read/act on FutScores.

- **To add:**
  - An MCP server exposing FutScores tools (e.g. `list_players`, `search`, `add_rating`).
    Cleanest path: a small server that calls the existing **REST API** (`/api/*`) so the
    business logic and auth are reused — no duplication of EF logic.
  - Place under a new top-level folder, e.g. `mcp-server/` (Node `@modelcontextprotocol/sdk`
    or a .NET MCP server), with tool definitions mapping 1:1 to API endpoints.
  - Document how to register it in an agentic IDE (config snippet) for the oral demo.

### 2.9 No-crash impression — 12 pts (ongoing)

- Driven by everything above being solid. The global exception handler
  (`app.UseExceptionHandler("/Home/Error")`) and `Views/Shared/Error.cshtml` already
  exist. Keep the test suites green and smoke-test the main flows before submission.

### 2.10 Oral code-understanding exam — 40 pts (no code)

- The largest single block. Be ready to explain: the layer flow (controller → repo →
  DTO → view), the `User` vs `AppUser` split, the `SmartAuth` JWT-vs-cookie policy scheme
  in `Program.cs`, the rating cascade + server-side validation, EF relationships/
  migrations, and each new feature added for this project.

---

## 3. Suggested build order

1. **Logging** (2.5) — small, and useful while building everything else.
2. **Global search** (2.4) — self-contained, reuses existing query patterns.
3. **AI data entry** (2.3) — new service + endpoint; reuses repositories + validation.
4. **MCP server** (2.8) — wraps the API; best done after search/AI exist as endpoints.
5. **Playwright tests** (2.2) — once endpoints are stable.
6. **Deploy** (2.1) — last, so we ship the finished app.
7. **Responsive polish** (2.6) + **CRUD/no-crash** smoke pass (2.7/2.9) throughout.

---

## 4. Conventions to keep (so new code matches the codebase)

- New data feature → add an entity (if needed) + **repository interface & impl** +
  register it in `Program.cs` DI + a **DTO/ViewModel** + a thin controller. Don't put
  EF queries in controllers.
- API returns **DTOs**, never entities. Mutations on League/Club/Player/Match/User are
  `[Authorize(Roles="Admin")]`; ratings are owner-or-Admin.
- Secrets (API keys, JWT key, Google) come from configuration/env/user-secrets — never
  hard-coded or committed.
- Comments in the existing code are partly **Croatian** (esp. tests/auth notes); match
  the surrounding file.
- After any change touching endpoints or repositories, run
  `dotnet test ProbaMala/ProbaMala.IntegrationTests`.
