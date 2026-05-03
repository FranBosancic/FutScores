# Sitemap

## Routing Summary

Application routing is configured in `ProbaMala/ProbaMala/Program.cs` with:
- attribute routing enabled through `app.MapControllers();`
- default conventional fallback route `{controller=Home}/{action=Index}/{id?}`

Primary navigation should use the English custom attribute routes listed below. Croatian routes are supported aliases. The default conventional route still exists as a fallback.

## Canonical Primary Routes

| URL | Controller | Action | Route Style | View |
| --- | --- | --- | --- | --- |
| `/` | `HomeController` | `Index()` | Attribute | `Views/Home/Index.cshtml` |
| `/dashboard` | `HomeController` | `Index()` | Attribute | `Views/Home/Index.cshtml` |
| `/error` | `HomeController` | `Error()` | Attribute | `Views/Shared/Error.cshtml` |
| `/leagues` | `LeagueController` | `Index()` | Attribute | `Views/League/Index.cshtml` |
| `/leagues/list` | `LeagueController` | `Index()` | Attribute | `Views/League/Index.cshtml` |
| `/leagues/{id:int}` | `LeagueController` | `Details(int id)` | Attribute | `Views/League/Details.cshtml` |
| `/leagues/details/{id:int}` | `LeagueController` | `Details(int id)` | Attribute | `Views/League/Details.cshtml` |
| `/clubs` | `ClubController` | `Index()` | Attribute | `Views/Club/Index.cshtml` |
| `/clubs/list` | `ClubController` | `Index()` | Attribute | `Views/Club/Index.cshtml` |
| `/clubs/{id:int}` | `ClubController` | `Details(int id)` | Attribute | `Views/Club/Details.cshtml` |
| `/clubs/details/{id:int}` | `ClubController` | `Details(int id)` | Attribute | `Views/Club/Details.cshtml` |
| `/players` | `PlayerController` | `Index()` | Attribute | `Views/Player/Index.cshtml` |
| `/players/list` | `PlayerController` | `Index()` | Attribute | `Views/Player/Index.cshtml` |
| `/players/{id:int}` | `PlayerController` | `Details(int id)` | Attribute | `Views/Player/Details.cshtml` |
| `/players/details/{id:int}` | `PlayerController` | `Details(int id)` | Attribute | `Views/Player/Details.cshtml` |
| `/matches` | `MatchController` | `Index()` | Attribute | `Views/Match/Index.cshtml` |
| `/matches/list` | `MatchController` | `Index()` | Attribute | `Views/Match/Index.cshtml` |
| `/matches/{id:int}` | `MatchController` | `Details(int id)` | Attribute | `Views/Match/Details.cshtml` |
| `/matches/details/{id:int}` | `MatchController` | `Details(int id)` | Attribute | `Views/Match/Details.cshtml` |
| `/ratings` | `RatingController` | `Index()` | Attribute | `Views/Rating/Index.cshtml` |
| `/ratings/list` | `RatingController` | `Index()` | Attribute | `Views/Rating/Index.cshtml` |
| `/ratings/{id:int}` | `RatingController` | `Details(int id)` | Attribute | `Views/Rating/Details.cshtml` |
| `/ratings/details/{id:int}` | `RatingController` | `Details(int id)` | Attribute | `Views/Rating/Details.cshtml` |
| `/users` | `UserController` | `Index()` | Attribute | `Views/User/Index.cshtml` |
| `/users/list` | `UserController` | `Index()` | Attribute | `Views/User/Index.cshtml` |
| `/users/{id:int}` | `UserController` | `Details(int id)` | Attribute | `Views/User/Details.cshtml` |
| `/users/details/{id:int}` | `UserController` | `Details(int id)` | Attribute | `Views/User/Details.cshtml` |

## Croatian Aliases

| URL | Controller | Action | Route Style | View |
| --- | --- | --- | --- | --- |
| `/nadzorna-ploca` | `HomeController` | `Index()` | Attribute alias | `Views/Home/Index.cshtml` |
| `/greska` | `HomeController` | `Error()` | Attribute alias | `Views/Shared/Error.cshtml` |
| `/lige` | `LeagueController` | `Index()` | Attribute alias | `Views/League/Index.cshtml` |
| `/lige/popis` | `LeagueController` | `Index()` | Attribute alias | `Views/League/Index.cshtml` |
| `/lige/{id:int}` | `LeagueController` | `Details(int id)` | Attribute alias | `Views/League/Details.cshtml` |
| `/lige/detalji/{id:int}` | `LeagueController` | `Details(int id)` | Attribute alias | `Views/League/Details.cshtml` |
| `/klubovi` | `ClubController` | `Index()` | Attribute alias | `Views/Club/Index.cshtml` |
| `/klubovi/popis` | `ClubController` | `Index()` | Attribute alias | `Views/Club/Index.cshtml` |
| `/klubovi/{id:int}` | `ClubController` | `Details(int id)` | Attribute alias | `Views/Club/Details.cshtml` |
| `/klubovi/detalji/{id:int}` | `ClubController` | `Details(int id)` | Attribute alias | `Views/Club/Details.cshtml` |
| `/igraci` | `PlayerController` | `Index()` | Attribute alias | `Views/Player/Index.cshtml` |
| `/igraci/popis` | `PlayerController` | `Index()` | Attribute alias | `Views/Player/Index.cshtml` |
| `/igraci/{id:int}` | `PlayerController` | `Details(int id)` | Attribute alias | `Views/Player/Details.cshtml` |
| `/igraci/detalji/{id:int}` | `PlayerController` | `Details(int id)` | Attribute alias | `Views/Player/Details.cshtml` |
| `/utakmice` | `MatchController` | `Index()` | Attribute alias | `Views/Match/Index.cshtml` |
| `/utakmice/popis` | `MatchController` | `Index()` | Attribute alias | `Views/Match/Index.cshtml` |
| `/utakmice/{id:int}` | `MatchController` | `Details(int id)` | Attribute alias | `Views/Match/Details.cshtml` |
| `/utakmice/detalji/{id:int}` | `MatchController` | `Details(int id)` | Attribute alias | `Views/Match/Details.cshtml` |
| `/ocjene` | `RatingController` | `Index()` | Attribute alias | `Views/Rating/Index.cshtml` |
| `/ocjene/popis` | `RatingController` | `Index()` | Attribute alias | `Views/Rating/Index.cshtml` |
| `/ocjene/{id:int}` | `RatingController` | `Details(int id)` | Attribute alias | `Views/Rating/Details.cshtml` |
| `/ocjene/detalji/{id:int}` | `RatingController` | `Details(int id)` | Attribute alias | `Views/Rating/Details.cshtml` |
| `/korisnici` | `UserController` | `Index()` | Attribute alias | `Views/User/Index.cshtml` |
| `/korisnici/popis` | `UserController` | `Index()` | Attribute alias | `Views/User/Index.cshtml` |
| `/korisnici/{id:int}` | `UserController` | `Details(int id)` | Attribute alias | `Views/User/Details.cshtml` |
| `/korisnici/detalji/{id:int}` | `UserController` | `Details(int id)` | Attribute alias | `Views/User/Details.cshtml` |

## Route Parameters And Constraints

| URL Pattern | Parameters | Notes |
| --- | --- | --- |
| `/lige/{id:int}`, `/lige/detalji/{id:int}`, `/leagues/{id:int}`, `/leagues/details/{id:int}` | `id` | Integer league identifier |
| `/klubovi/{id:int}`, `/klubovi/detalji/{id:int}`, `/clubs/{id:int}`, `/clubs/details/{id:int}` | `id` | Integer club identifier |
| `/igraci/{id:int}`, `/igraci/detalji/{id:int}`, `/players/{id:int}`, `/players/details/{id:int}` | `id` | Integer player identifier |
| `/utakmice/{id:int}`, `/utakmice/detalji/{id:int}`, `/matches/{id:int}`, `/matches/details/{id:int}` | `id` | Integer match identifier |
| `/ocjene/{id:int}`, `/ocjene/detalji/{id:int}`, `/ratings/{id:int}`, `/ratings/details/{id:int}` | `id` | Integer rating identifier |
| `/korisnici/{id:int}`, `/korisnici/detalji/{id:int}`, `/users/{id:int}`, `/users/details/{id:int}` | `id` | Integer user identifier |

## Canonical Preference

- English routes are the primary public-facing URLs.
- Croatian routes are first-class aliases and should keep working.
- For detail pages, the short English form is the preferred route, for example `/matches/5`.
- Explicit detail routes such as `/matches/details/5` remain valid aliases when a more descriptive URL is useful.

## Default Conventional Fallback

The following fallback pattern is still active:

`/{controller=Home}/{action=Index}/{id?}`

This means these URLs also remain reachable:
- `/Home/Index`
- `/Home/Error`
- `/League/Index`
- `/League/Details/1`
- `/Club/Index`
- `/Club/Details/1`
- `/Player/Index`
- `/Player/Details/1`
- `/Match/Index`
- `/Match/Details/1`
- `/Rating/Index`
- `/Rating/Details/1`
- `/User/Index`
- `/User/Details/1`

## Notes

- The custom attribute routes support both English canonical URLs and Croatian aliases.
- Detail pages support both short and explicit detail URLs, for example `/matches/5` and `/matches/details/5`.
- Razor links in the app use tag helpers such as `asp-controller` and `asp-action`, so navigation resolves through the active routing table.
- The lab requirement for custom routing is satisfied through the attribute-routed controller actions above.