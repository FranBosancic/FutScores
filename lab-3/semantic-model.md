# Semantic model – FutScores

Kratki pregled svih entiteta, njihovih glavnih svojstava i veza između tablica.

---

## Entiteti

### League (Liga)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| Name | string | Naziv lige (npr. "Premier League") |

Veze:
- 1-N prema **Club** (liga ima više klubova)
- 1-N prema **Match** (liga ima više utakmica)

---

### Club (Klub)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| Name | string | Naziv kluba |
| FoundedDate | DateTime | Datum osnivanja |
| LeagueId | int | FK → League |

Veze:
- N-1 prema **League**
- 1-N prema **Player** (klub ima više igrača)
- 1-N prema **Match** kao domaćin (HomeTeam)
- 1-N prema **Match** kao gost (AwayTeam)

---

### Player (Igrač)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| FirstName | string | Ime |
| LastName | string | Prezime |
| DateOfBirth | DateTime | Datum rođenja |
| Position | Position (enum) | Pozicija na terenu |
| ClubId | int | FK → Club |
| Nationality | string | Nacionalnost |

Veze:
- N-1 prema **Club**
- 1-N prema **Rating** (igrač ima više ocjena)

---

### Match (Utakmica)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| LeagueId | int | FK → League |
| HomeTeamId | int | FK → Club (domaćin) |
| AwayTeamId | int | FK → Club (gost) |
| Date | DateTime | Datum i vrijeme utakmice |
| HomeGoals | int | Golovi domaćina |
| AwayGoals | int | Golovi gosta |

Veze:
- N-1 prema **League**
- N-1 prema **Club** (domaćin)
- N-1 prema **Club** (gost)
- 1-N prema **Rating**

---

### Rating (Ocjena igrača)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| PlayerId | int | FK → Player |
| MatchId | int | FK → Match |
| UserId | int | FK → User |
| Score | int | Ocjena 1–10 |
| Comment | string? | Opcionalni komentar |

Veze:
- N-1 prema **Player**
- N-1 prema **Match**
- N-1 prema **User**

---

### User (Korisnik koji ocjenjuje)
| Svojstvo | Tip | Napomena |
|---|---|---|
| Id | int | Primarni ključ |
| FirstName | string | Ime |
| LastName | string | Prezime |
| Email | string | Email adresa |

Veze:
- 1-N prema **Rating**

---

### Position (enum)
```
Goalkeeper  = 0
Defender    = 1
Midfielder  = 2
Forward     = 3
```

---

## Dijagram veza

```
League ──< Club ──< Player ──< Rating
   └──< Match ──────────────┘
              └── User ──────┘
```

Klub je povezan s utakmicom na **dva mjesta**: jednom kao domaćin i jednom kao gost.
Zbog toga su na entitetu `Match` dva strana ključa koja oba pokazuju na `Club`,
a na entitetu `Club` postoje dvije zasebne kolekcije (`HomeMatches`, `AwayMatches`).
