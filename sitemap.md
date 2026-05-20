# Sitemap

## Routing Summary

Application routing is configured in `ProbaMala/ProbaMala/Program.cs` with:
- attribute routing enabled through `app.MapControllers();`
- default conventional fallback route `{controller=Home}/{action=Index}/{id?}`

Primary navigation uses English custom routes as canonical URLs.
Croatian routes remain supported aliases for the same actions.
Create, edit, and delete pages also have matching POST endpoints on the same URL patterns.

## Canonical Primary Routes

| URL | Controller | Action | Route Style | View |
| --- | --- | --- | --- | --- |
| `/` | `HomeController` | `Index()` | Attribute | `Views/Home/Index.cshtml` |
| `/dashboard` | `HomeController` | `Index()` | Attribute | `Views/Home/Index.cshtml` |
| `/error` | `HomeController` | `Error()` | Attribute | `Views/Shared/Error.cshtml` |
| `/leagues` | `LeagueController` | `Index(string? q)` | Attribute | `Views/League/Index.cshtml` |
| `/leagues/list` | `LeagueController` | `Index(string? q)` | Attribute | `Views/League/Index.cshtml` |
| `/leagues/filter` | `LeagueController` | `Filter(string? q)` | Attribute | `Views/League/_LeagueList.cshtml` |
| `/leagues/{id:int}` | `LeagueController` | `Details(int id)` | Attribute | `Views/League/Details.cshtml` |
| `/leagues/details/{id:int}` | `LeagueController` | `Details(int id)` | Attribute | `Views/League/Details.cshtml` |
| `/leagues/create` | `LeagueController` | `Create()` | Attribute | `Views/League/Create.cshtml` |
| `/leagues/edit/{id:int}` | `LeagueController` | `Edit(int id)` | Attribute | `Views/League/Edit.cshtml` |
| `/leagues/delete/{id:int}` | `LeagueController` | `Delete(int id)` | Attribute | `Views/League/Delete.cshtml` |
| `/clubs` | `ClubController` | `Index(string? q)` | Attribute | `Views/Club/Index.cshtml` |
| `/clubs/list` | `ClubController` | `Index(string? q)` | Attribute | `Views/Club/Index.cshtml` |
| `/clubs/filter` | `ClubController` | `Filter(string? q)` | Attribute | `Views/Club/_ClubList.cshtml` |
| `/clubs/{id:int}` | `ClubController` | `Details(int id)` | Attribute | `Views/Club/Details.cshtml` |
| `/clubs/details/{id:int}` | `ClubController` | `Details(int id)` | Attribute | `Views/Club/Details.cshtml` |
| `/clubs/create` | `ClubController` | `Create()` | Attribute | `Views/Club/Create.cshtml` |
| `/clubs/edit/{id:int}` | `ClubController` | `Edit(int id)` | Attribute | `Views/Club/Edit.cshtml` |
| `/clubs/delete/{id:int}` | `ClubController` | `Delete(int id)` | Attribute | `Views/Club/Delete.cshtml` |
| `/players` | `PlayerController` | `Index(string? q)` | Attribute | `Views/Player/Index.cshtml` |
| `/players/list` | `PlayerController` | `Index(string? q)` | Attribute | `Views/Player/Index.cshtml` |
| `/players/filter` | `PlayerController` | `Filter(string? q)` | Attribute | `Views/Player/_PlayerList.cshtml` |
| `/players/{id:int}` | `PlayerController` | `Details(int id)` | Attribute | `Views/Player/Details.cshtml` |
| `/players/details/{id:int}` | `PlayerController` | `Details(int id)` | Attribute | `Views/Player/Details.cshtml` |
| `/players/create` | `PlayerController` | `Create()` | Attribute | `Views/Player/Create.cshtml` |
| `/players/edit/{id:int}` | `PlayerController` | `Edit(int id)` | Attribute | `Views/Player/Edit.cshtml` |
| `/players/delete/{id:int}` | `PlayerController` | `Delete(int id)` | Attribute | `Views/Player/Delete.cshtml` |
| `/matches` | `MatchController` | `Index(string? q)` | Attribute | `Views/Match/Index.cshtml` |
| `/matches/list` | `MatchController` | `Index(string? q)` | Attribute | `Views/Match/Index.cshtml` |
| `/matches/filter` | `MatchController` | `Filter(string? q)` | Attribute | `Views/Match/_MatchList.cshtml` |
| `/matches/{id:int}` | `MatchController` | `Details(int id)` | Attribute | `Views/Match/Details.cshtml` |
| `/matches/details/{id:int}` | `MatchController` | `Details(int id)` | Attribute | `Views/Match/Details.cshtml` |
| `/matches/create` | `MatchController` | `Create()` | Attribute | `Views/Match/Create.cshtml` |
| `/matches/edit/{id:int}` | `MatchController` | `Edit(int id)` | Attribute | `Views/Match/Edit.cshtml` |
| `/matches/delete/{id:int}` | `MatchController` | `Delete(int id)` | Attribute | `Views/Match/Delete.cshtml` |
| `/ratings` | `RatingController` | `Index(string? q)` | Attribute | `Views/Rating/Index.cshtml` |
| `/ratings/list` | `RatingController` | `Index(string? q)` | Attribute | `Views/Rating/Index.cshtml` |
| `/ratings/filter` | `RatingController` | `Filter(string? q)` | Attribute | `Views/Rating/_RatingList.cshtml` |
| `/ratings/{id:int}` | `RatingController` | `Details(int id)` | Attribute | `Views/Rating/Details.cshtml` |
| `/ratings/details/{id:int}` | `RatingController` | `Details(int id)` | Attribute | `Views/Rating/Details.cshtml` |
| `/ratings/create` | `RatingController` | `Create()` | Attribute | `Views/Rating/Create.cshtml` |
| `/ratings/edit/{id:int}` | `RatingController` | `Edit(int id)` | Attribute | `Views/Rating/Edit.cshtml` |
| `/ratings/delete/{id:int}` | `RatingController` | `Delete(int id)` | Attribute | `Views/Rating/Delete.cshtml` |
| `/ratings/players/search` | `RatingController` | `SearchPlayers(string? q)` | Attribute | JSON response |
| `/users` | `UserController` | `Index(string? q)` | Attribute | `Views/User/Index.cshtml` |
| `/users/list` | `UserController` | `Index(string? q)` | Attribute | `Views/User/Index.cshtml` |
| `/users/filter` | `UserController` | `Filter(string? q)` | Attribute | `Views/User/_UserList.cshtml` |
| `/users/{id:int}` | `UserController` | `Details(int id)` | Attribute | `Views/User/Details.cshtml` |
| `/users/details/{id:int}` | `UserController` | `Details(int id)` | Attribute | `Views/User/Details.cshtml` |
| `/users/create` | `UserController` | `Create()` | Attribute | `Views/User/Create.cshtml` |
| `/users/edit/{id:int}` | `UserController` | `Edit(int id)` | Attribute | `Views/User/Edit.cshtml` |
| `/users/delete/{id:int}` | `UserController` | `Delete(int id)` | Attribute | `Views/User/Delete.cshtml` |

## Croatian Aliases

| URL | Controller | Action | Route Style | View |
| --- | --- | --- | --- | --- |
| `/nadzorna-ploca` | `HomeController` | `Index()` | Attribute alias | `Views/Home/Index.cshtml` |
| `/greska` | `HomeController` | `Error()` | Attribute alias | `Views/Shared/Error.cshtml` |
| `/lige` | `LeagueController` | `Index(string? q)` | Attribute alias | `Views/League/Index.cshtml` |
| `/lige/popis` | `LeagueController` | `Index(string? q)` | Attribute alias | `Views/League/Index.cshtml` |
| `/lige/filter` | `LeagueController` | `Filter(string? q)` | Attribute alias | `Views/League/_LeagueList.cshtml` |
| `/lige/{id:int}` | `LeagueController` | `Details(int id)` | Attribute alias | `Views/League/Details.cshtml` |
| `/lige/detalji/{id:int}` | `LeagueController` | `Details(int id)` | Attribute alias | `Views/League/Details.cshtml` |
| `/lige/novo` | `LeagueController` | `Create()` | Attribute alias | `Views/League/Create.cshtml` |
| `/lige/uredi/{id:int}` | `LeagueController` | `Edit(int id)` | Attribute alias | `Views/League/Edit.cshtml` |
| `/lige/obrisi/{id:int}` | `LeagueController` | `Delete(int id)` | Attribute alias | `Views/League/Delete.cshtml` |
| `/klubovi` | `ClubController` | `Index(string? q)` | Attribute alias | `Views/Club/Index.cshtml` |
| `/klubovi/popis` | `ClubController` | `Index(string? q)` | Attribute alias | `Views/Club/Index.cshtml` |
| `/klubovi/filter` | `ClubController` | `Filter(string? q)` | Attribute alias | `Views/Club/_ClubList.cshtml` |
| `/klubovi/{id:int}` | `ClubController` | `Details(int id)` | Attribute alias | `Views/Club/Details.cshtml` |
| `/klubovi/detalji/{id:int}` | `ClubController` | `Details(int id)` | Attribute alias | `Views/Club/Details.cshtml` |
| `/klubovi/novo` | `ClubController` | `Create()` | Attribute alias | `Views/Club/Create.cshtml` |
| `/klubovi/uredi/{id:int}` | `ClubController` | `Edit(int id)` | Attribute alias | `Views/Club/Edit.cshtml` |
| `/klubovi/obrisi/{id:int}` | `ClubController` | `Delete(int id)` | Attribute alias | `Views/Club/Delete.cshtml` |
| `/igraci` | `PlayerController` | `Index(string? q)` | Attribute alias | `Views/Player/Index.cshtml` |
| `/igraci/popis` | `PlayerController` | `Index(string? q)` | Attribute alias | `Views/Player/Index.cshtml` |
| `/igraci/filter` | `PlayerController` | `Filter(string? q)` | Attribute alias | `Views/Player/_PlayerList.cshtml` |
| `/igraci/{id:int}` | `PlayerController` | `Details(int id)` | Attribute alias | `Views/Player/Details.cshtml` |
| `/igraci/detalji/{id:int}` | `PlayerController` | `Details(int id)` | Attribute alias | `Views/Player/Details.cshtml` |
| `/igraci/novo` | `PlayerController` | `Create()` | Attribute alias | `Views/Player/Create.cshtml` |
| `/igraci/uredi/{id:int}` | `PlayerController` | `Edit(int id)` | Attribute alias | `Views/Player/Edit.cshtml` |
| `/igraci/obrisi/{id:int}` | `PlayerController` | `Delete(int id)` | Attribute alias | `Views/Player/Delete.cshtml` |
| `/utakmice` | `MatchController` | `Index(string? q)` | Attribute alias | `Views/Match/Index.cshtml` |
| `/utakmice/popis` | `MatchController` | `Index(string? q)` | Attribute alias | `Views/Match/Index.cshtml` |
| `/utakmice/filter` | `MatchController` | `Filter(string? q)` | Attribute alias | `Views/Match/_MatchList.cshtml` |
| `/utakmice/{id:int}` | `MatchController` | `Details(int id)` | Attribute alias | `Views/Match/Details.cshtml` |
| `/utakmice/detalji/{id:int}` | `MatchController` | `Details(int id)` | Attribute alias | `Views/Match/Details.cshtml` |
| `/utakmice/novo` | `MatchController` | `Create()` | Attribute alias | `Views/Match/Create.cshtml` |
| `/utakmice/uredi/{id:int}` | `MatchController` | `Edit(int id)` | Attribute alias | `Views/Match/Edit.cshtml` |
| `/utakmice/obrisi/{id:int}` | `MatchController` | `Delete(int id)` | Attribute alias | `Views/Match/Delete.cshtml` |
| `/ocjene` | `RatingController` | `Index(string? q)` | Attribute alias | `Views/Rating/Index.cshtml` |
| `/ocjene/popis` | `RatingController` | `Index(string? q)` | Attribute alias | `Views/Rating/Index.cshtml` |
| `/ocjene/filter` | `RatingController` | `Filter(string? q)` | Attribute alias | `Views/Rating/_RatingList.cshtml` |
| `/ocjene/{id:int}` | `RatingController` | `Details(int id)` | Attribute alias | `Views/Rating/Details.cshtml` |
| `/ocjene/detalji/{id:int}` | `RatingController` | `Details(int id)` | Attribute alias | `Views/Rating/Details.cshtml` |
| `/ocjene/novo` | `RatingController` | `Create()` | Attribute alias | `Views/Rating/Create.cshtml` |
| `/ocjene/uredi/{id:int}` | `RatingController` | `Edit(int id)` | Attribute alias | `Views/Rating/Edit.cshtml` |
| `/ocjene/obrisi/{id:int}` | `RatingController` | `Delete(int id)` | Attribute alias | `Views/Rating/Delete.cshtml` |
| `/ocjene/igraci/pretraga` | `RatingController` | `SearchPlayers(string? q)` | Attribute alias | JSON response |
| `/korisnici` | `UserController` | `Index(string? q)` | Attribute alias | `Views/User/Index.cshtml` |
| `/korisnici/popis` | `UserController` | `Index(string? q)` | Attribute alias | `Views/User/Index.cshtml` |
| `/korisnici/filter` | `UserController` | `Filter(string? q)` | Attribute alias | `Views/User/_UserList.cshtml` |
| `/korisnici/{id:int}` | `UserController` | `Details(int id)` | Attribute alias | `Views/User/Details.cshtml` |
| `/korisnici/detalji/{id:int}` | `UserController` | `Details(int id)` | Attribute alias | `Views/User/Details.cshtml` |
| `/korisnici/novo` | `UserController` | `Create()` | Attribute alias | `Views/User/Create.cshtml` |
| `/korisnici/uredi/{id:int}` | `UserController` | `Edit(int id)` | Attribute alias | `Views/User/Edit.cshtml` |
| `/korisnici/obrisi/{id:int}` | `UserController` | `Delete(int id)` | Attribute alias | `Views/User/Delete.cshtml` |

## Route Parameters And Constraints

| URL Pattern | Parameters | Notes |
| --- | --- | --- |
| `/lige/{id:int}`, `/lige/detalji/{id:int}`, `/leagues/{id:int}`, `/leagues/details/{id:int}` | `id` | Integer league identifier |
| `/lige/uredi/{id:int}`, `/lige/obrisi/{id:int}`, `/leagues/edit/{id:int}`, `/leagues/delete/{id:int}` | `id` | Integer league identifier for form pages |
| `/klubovi/{id:int}`, `/klubovi/detalji/{id:int}`, `/clubs/{id:int}`, `/clubs/details/{id:int}` | `id` | Integer club identifier |
| `/klubovi/uredi/{id:int}`, `/klubovi/obrisi/{id:int}`, `/clubs/edit/{id:int}`, `/clubs/delete/{id:int}` | `id` | Integer club identifier for form pages |
| `/igraci/{id:int}`, `/igraci/detalji/{id:int}`, `/players/{id:int}`, `/players/details/{id:int}` | `id` | Integer player identifier |
| `/igraci/uredi/{id:int}`, `/igraci/obrisi/{id:int}`, `/players/edit/{id:int}`, `/players/delete/{id:int}` | `id` | Integer player identifier for form pages |
| `/utakmice/{id:int}`, `/utakmice/detalji/{id:int}`, `/matches/{id:int}`, `/matches/details/{id:int}` | `id` | Integer match identifier |
| `/utakmice/uredi/{id:int}`, `/utakmice/obrisi/{id:int}`, `/matches/edit/{id:int}`, `/matches/delete/{id:int}` | `id` | Integer match identifier for form pages |
| `/ocjene/{id:int}`, `/ocjene/detalji/{id:int}`, `/ratings/{id:int}`, `/ratings/details/{id:int}` | `id` | Integer rating identifier |
| `/ocjene/uredi/{id:int}`, `/ocjene/obrisi/{id:int}`, `/ratings/edit/{id:int}`, `/ratings/delete/{id:int}` | `id` | Integer rating identifier for form pages |
| `/korisnici/{id:int}`, `/korisnici/detalji/{id:int}`, `/users/{id:int}`, `/users/details/{id:int}` | `id` | Integer user identifier |
| `/korisnici/uredi/{id:int}`, `/korisnici/obrisi/{id:int}`, `/users/edit/{id:int}`, `/users/delete/{id:int}` | `id` | Integer user identifier for form pages |
| `/lige/filter`, `/klubovi/filter`, `/igraci/filter`, `/utakmice/filter`, `/ocjene/filter`, `/korisnici/filter` | `q` | Optional AJAX filter query |
| `/leagues/filter`, `/clubs/filter`, `/players/filter`, `/matches/filter`, `/ratings/filter`, `/users/filter` | `q` | Optional AJAX filter query |
| `/ocjene/igraci/pretraga`, `/ratings/players/search` | `q` | Optional autocomplete search query |

## Canonical Preference

- English routes are the primary public-facing URLs.
- Croatian routes are first-class aliases and should keep working.
- For detail pages, the short English form is preferred, for example `/matches/5`.
- Explicit detail routes such as `/matches/details/5` remain valid aliases.
- Form pages use English canonical URLs such as `/clubs/create`, `/clubs/edit/4`, and `/clubs/delete/4`.

## Default Conventional Fallback

The following fallback pattern is still active:

`/{controller=Home}/{action=Index}/{id?}`

This means these URLs also remain reachable:
- `/Home/Index`
- `/Home/Error`
- `/League/Index`
- `/League/Details/1`
- `/League/Create`
- `/League/Edit/1`
- `/League/Delete/1`
- `/Club/Index`
- `/Club/Details/1`
- `/Club/Create`
- `/Club/Edit/1`
- `/Club/Delete/1`
- `/Player/Index`
- `/Player/Details/1`
- `/Player/Create`
- `/Player/Edit/1`
- `/Player/Delete/1`
- `/Match/Index`
- `/Match/Details/1`
- `/Match/Create`
- `/Match/Edit/1`
- `/Match/Delete/1`
- `/Rating/Index`
- `/Rating/Details/1`
- `/Rating/Create`
- `/Rating/Edit/1`
- `/Rating/Delete/1`
- `/User/Index`
- `/User/Details/1`
- `/User/Create`
- `/User/Edit/1`
- `/User/Delete/1`

## Notes

- The custom attribute routes support both English canonical URLs and Croatian aliases.
- CRUD form pages now have explicit custom routes in both routing families.
- AJAX filtering endpoints are documented because they are part of the reachable routing surface.
- The player autocomplete endpoint for ratings is documented as a JSON helper route.
- Razor navigation typically uses named routes such as `leagues-index`, `club-details`, `rating-edit`, and `users-filter`.
- The lab requirement for custom routing is satisfied well beyond the minimum through the attribute-routed actions above.