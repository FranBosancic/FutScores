# Sitemap – FutScores

Popis svih dostupnih URL adresa u aplikaciji s informacijom koji controller, koja akcija i koji view se koriste.

---

## HomeController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/` ili `/home` | GET | `Index` | `Home/Index.cshtml` |

---

## LeagueController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/leagues` ili `/lige` | GET | `Index` | `League/Index.cshtml` (partial: `_LeagueList`) |
| `/leagues/filter` | GET | `Filter` | `_LeagueList` (partial, AJAX) |
| `/leagues/{id}` | GET | `Details` | `League/Details.cshtml` |
| `/leagues/create` | GET | `Create` | `League/Create.cshtml` (partial: `_LeagueForm`) |
| `/leagues/create` | POST | `Create` | redirect → Details |
| `/leagues/edit/{id}` | GET | `Edit` | `League/Edit.cshtml` (partial: `_LeagueForm`) |
| `/leagues/edit/{id}` | POST | `Edit` | redirect → Details |
| `/leagues/delete/{id}` | GET | `Delete` | `League/Delete.cshtml` |
| `/leagues/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## ClubController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/clubs` ili `/klubovi` | GET | `Index` | `Club/Index.cshtml` (partial: `_ClubList`) |
| `/clubs/filter` | GET | `Filter` | `_ClubList` (partial, AJAX) |
| `/clubs/{id}` | GET | `Details` | `Club/Details.cshtml` |
| `/clubs/create` | GET | `Create` | `Club/Create.cshtml` (partial: `_ClubForm`) |
| `/clubs/create` | POST | `Create` | redirect → Details |
| `/clubs/edit/{id}` | GET | `Edit` | `Club/Edit.cshtml` (partial: `_ClubForm`) |
| `/clubs/edit/{id}` | POST | `Edit` | redirect → Details |
| `/clubs/delete/{id}` | GET | `Delete` | `Club/Delete.cshtml` |
| `/clubs/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## PlayerController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/players` ili `/igraci` | GET | `Index` | `Player/Index.cshtml` (partial: `_PlayerList`) |
| `/players/filter` | GET | `Filter` | `_PlayerList` (partial, AJAX) |
| `/players/{id}` | GET | `Details` | `Player/Details.cshtml` |
| `/players/create` | GET | `Create` | `Player/Create.cshtml` (partial: `_PlayerForm`) |
| `/players/create` | POST | `Create` | redirect → Details |
| `/players/edit/{id}` | GET | `Edit` | `Player/Edit.cshtml` (partial: `_PlayerForm`) |
| `/players/edit/{id}` | POST | `Edit` | redirect → Details |
| `/players/delete/{id}` | GET | `Delete` | `Player/Delete.cshtml` |
| `/players/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## MatchController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/matches` ili `/utakmice` | GET | `Index` | `Match/Index.cshtml` (partial: `_MatchList`) |
| `/matches/filter` | GET | `Filter` | `_MatchList` (partial, AJAX) |
| `/matches/{id}` | GET | `Details` | `Match/Details.cshtml` |
| `/matches/create` | GET | `Create` | `Match/Create.cshtml` (partial: `_MatchForm`) |
| `/matches/clubs` | GET | `ClubsInLeague` | JSON odgovor (cascade AJAX) |
| `/matches/create` | POST | `Create` | redirect → Details |
| `/matches/edit/{id}` | GET | `Edit` | `Match/Edit.cshtml` (partial: `_MatchForm`) |
| `/matches/edit/{id}` | POST | `Edit` | redirect → Details |
| `/matches/delete/{id}` | GET | `Delete` | `Match/Delete.cshtml` |
| `/matches/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## RatingController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/ratings` ili `/ocjene` | GET | `Index` | `Rating/Index.cshtml` (partial: `_RatingList`) |
| `/ratings/filter` | GET | `Filter` | `_RatingList` (partial, AJAX) |
| `/ratings/{id}` | GET | `Details` | `Rating/Details.cshtml` |
| `/ratings/create` | GET | `Create` | `Rating/Create.cshtml` (partial: `_RatingForm`) |
| `/ratings/clubs` | GET | `ClubsInLeague` | JSON odgovor (cascade AJAX) |
| `/ratings/matches` | GET | `MatchesBetween` | JSON odgovor (cascade AJAX) |
| `/ratings/players` | GET | `PlayersForMatch` | JSON odgovor (cascade AJAX) |
| `/ratings/create` | POST | `Create` | redirect → Details |
| `/ratings/edit/{id}` | GET | `Edit` | `Rating/Edit.cshtml` (partial: `_RatingForm`) |
| `/ratings/edit/{id}` | POST | `Edit` | redirect → Details |
| `/ratings/delete/{id}` | GET | `Delete` | `Rating/Delete.cshtml` |
| `/ratings/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## UserController

| URL | Metoda | Akcija | View |
|---|---|---|---|
| `/users` ili `/korisnici` | GET | `Index` | `User/Index.cshtml` (partial: `_UserList`) |
| `/users/filter` | GET | `Filter` | `_UserList` (partial, AJAX) |
| `/users/{id}` | GET | `Details` | `User/Details.cshtml` |
| `/users/create` | GET | `Create` | `User/Create.cshtml` (partial: `_UserForm`) |
| `/users/create` | POST | `Create` | redirect → Details |
| `/users/edit/{id}` | GET | `Edit` | `User/Edit.cshtml` (partial: `_UserForm`) |
| `/users/edit/{id}` | POST | `Edit` | redirect → Details |
| `/users/delete/{id}` | GET | `Delete` | `User/Delete.cshtml` |
| `/users/delete/{id}` | POST | `Delete` (ActionName) | redirect → Index |

---

## Napomene o rutiranju

Svaki controller ima dvije varijante URL-a:
- Hrvatska verzija (npr. `/utakmice`, `/igraci`) — definirana kao `[Route]` atribut na controlleru
- Engleski alias (npr. `/matches`, `/players`) — definiran uz svaku akciju kao `[HttpGet("~/matches/...")]`

Oba URL-a vode na istu akciju. Engleski URL je definiran kao **named route** (npr. `Name = "matches-index"`) jer se koristi u views-ovima kroz `asp-route="matches-index"`.
