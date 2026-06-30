---
name: futscores-feature
description: Use this skill when adding or changing a feature in the FutScores (ProbaMala) ASP.NET Core 8 MVC + REST API football app — new entity, endpoint, repository, page, search, AI data entry, logging, or any change that must follow the project's layered architecture. Keywords: FutScores, ProbaMala, ASP.NET Core, MVC, Web API, EF Core, repository, DTO, ViewModel, controller, Razor, PostgreSQL, Identity, JWT, migration.
---

# FutScores Feature Development

Use this skill to make changes in the FutScores app the way the existing code is
built, so new work matches conventions and the oral exam stays defensible. The project
is `ProbaMala/ProbaMala`. See [`PROJECT-PLAN.md`](../../../PROJECT-PLAN.md) at the repo
root for the requirement-to-code map.

## Architecture (the request flow)

`Controller` (thin) → `I<Entity>Repository` (injected) → EF queries in the repo →
map entity to **DTO** (API) or **ViewModel** (MVC) → return to view or serialize.

Never put EF queries in a controller. API never returns raw entities — always DTOs.

## Folder responsibilities (under `ProbaMala/ProbaMala/`)

- `Models/Entities/` — EF entities (`League`, `Club`, `Player`, `Match`, `Rating`,
  `User`, `Image`, `AppUser`, `Position`). Annotate keys/FKs; configure relationships
  in `Data/AppDbContext.OnModelCreating`.
- `Models/DTOs/` — API request/response shapes.
- `Models/ViewModels/` — Razor form/details models.
- `Repositories/` — one `I<Entity>Repository` + `<Entity>Repository` per aggregate;
  all data access lives here. Register every repo in `Program.cs` (`AddScoped`).
- `Controllers/` — MVC page controllers (Croatian `[Route]`s, return Views).
- `Controllers/Api/` — `[ApiController]`, route `api/<entity>`, return DTOs.
- `Services/` — cross-cutting services (e.g. `JwtTokenService`).
- `Views/<Entity>/` — `Index/Details/Create/Edit/Delete` + `_<Entity>Form` /
  `_<Entity>List` partials; shared bits in `Views/Shared/`.
- `Migrations/` — EF migrations.
- `Data/` — `AppDbContext` (`IdentityDbContext<AppUser>`) + `IdentitySeeder`.

## Key domain facts (don't get these wrong)

- `User` (rating author, **int** key) is separate from `AppUser` (Identity login,
  **string** key). They link via `User.AppUserId` (nullable, 1:1, OnDelete SetNull).
- A `Match` has two FKs to `Club` (`HomeTeamId`/`AwayTeamId`) with `Restrict` delete;
  `Image` belongs to either a Club or a Player (one nullable FK set), cascade delete.
- Auth: `Program.cs` uses a `SmartAuth` policy scheme — `Authorization: Bearer` →
  JWT, otherwise Identity cookie — so MVC and API share `[Authorize]`. League/Club/
  Player/Match/User mutations are `[Authorize(Roles="Admin")]`; Rating create is any
  signed-in user, edit/delete is owner-or-Admin. GET endpoints are public.

## Steps to add a typical feature

1. (If new data) add/extend an entity in `Models/Entities/`, configure it in
   `AppDbContext`, then `dotnet ef migrations add <Name>` and `database update`.
2. Add `I<Entity>Repository` + impl in `Repositories/`; register in `Program.cs`.
3. Add DTO(s) in `Models/DTOs/` and/or ViewModel(s) in `Models/ViewModels/`.
4. Add a thin controller (`Controllers/` for pages, `Controllers/Api/` for REST).
   Apply the auth rules above.
5. Add/extend views + partials; wire any AJAX in `wwwroot/js/site.js`.
6. Run `dotnet test ProbaMala/ProbaMala.IntegrationTests` (114 tests) — keep it green;
   add tests for new endpoints following the existing `*ApiTests.cs` pattern.

## Conventions

- Secrets (API keys, `Jwt:Key`, Google) come from configuration / env vars /
  user-secrets — never hard-coded or committed.
- Match surrounding comment language — parts of the code (tests, auth notes) are in
  **Croatian**.
- Reuse existing validation (e.g. `RatingController.ValidateRatingForm`, repository
  consistency checks) instead of re-implementing it.
- UI is Tailwind (CDN + generated CSS) with a desktop nav and a separate mobile menu in
  `Views/Shared/_Layout.cshtml`; keep new UI responsive.

## Do not

- Put business/EF logic in controllers or views.
- Return entities from the API.
- Add a new dependency without a real need (the lab grades understanding — keep it
  explainable).
