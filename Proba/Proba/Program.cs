using Proba.Models;

// ===== INITIALIZING DATA =====
Console.WriteLine("=== FutScores - Football Match and Player Performance Rating Platform ===\n");

// Create Leagues
var premierLeague = new League("Premier League", "England", 1992, 20)
{
    LeagueId = 1
};

var laLiga = new League("La Liga", "Spain", 1929, 20)
{
    LeagueId = 2
};

var bundesliga = new League("Bundesliga", "Germany", 1963, 18)
{
    LeagueId = 3
};

var allLeagues = new List<League> { premierLeague, laLiga, bundesliga };

// ===== PREMIER LEAGUE - CLUBS AND PLAYERS =====
var manchesterCity = new Club("Manchester City", "Manchester", 1880, "Etihad Stadium", premierLeague)
{
    ClubId = 1,
    Points = 89,
    LeagueId = 1
};

var manchesterCity_Players = new List<Player>
{
    new Player("Ederson", 1, new DateTime(1993, 8, 17), PlayerPosition.Goalkeeper, manchesterCity) { PlayerId = 1, Appearances = 38, ClubId = 1 },
    new Player("Kyle Walker", 2, new DateTime(1990, 5, 28), PlayerPosition.Defender, manchesterCity) { PlayerId = 2, Appearances = 35, ClubId = 1 },
    new Player("Rodri", 16, new DateTime(1996, 6, 22), PlayerPosition.Midfielder, manchesterCity) { PlayerId = 3, Appearances = 37, ClubId = 1 },
    new Player("Erling Haaland", 9, new DateTime(2000, 7, 21), PlayerPosition.Forward, manchesterCity) { PlayerId = 4, Appearances = 36, ClubId = 1 }
};
manchesterCity.Players = manchesterCity_Players;

var liverpool = new Club("Liverpool", "Liverpool", 1892, "Anfield", premierLeague)
{
    ClubId = 2,
    Points = 82,
    LeagueId = 1
};

var liverpool_Players = new List<Player>
{
    new Player("Alisson", 1, new DateTime(1992, 10, 2), PlayerPosition.Goalkeeper, liverpool) { PlayerId = 5, Appearances = 38, ClubId = 2 },
    new Player("Trent Alexander-Arnold", 66, new DateTime(1998, 10, 7), PlayerPosition.Defender, liverpool) { PlayerId = 6, Appearances = 32, ClubId = 2 },
    new Player("Mohamed Salah", 11, new DateTime(1992, 6, 15), PlayerPosition.Forward, liverpool) { PlayerId = 7, Appearances = 37, ClubId = 2 },
    new Player("Alexis Mac Allister", 10, new DateTime(1998, 7, 24), PlayerPosition.Midfielder, liverpool) { PlayerId = 8, Appearances = 28, ClubId = 2 }
};
liverpool.Players = liverpool_Players;

var chelsea = new Club("Chelsea", "London", 1905, "Stamford Bridge", premierLeague)
{
    ClubId = 3,
    Points = 76,
    LeagueId = 1
};

var chelsea_Players = new List<Player>
{
    new Player("Robert Sánchez", 1, new DateTime(1996, 11, 18), PlayerPosition.Goalkeeper, chelsea) { PlayerId = 9, Appearances = 30, ClubId = 3 },
    new Player("Reece James", 24, new DateTime(1999, 12, 8), PlayerPosition.Defender, chelsea) { PlayerId = 10, Appearances = 25, ClubId = 3 },
    new Player("Conor Gallagher", 23, new DateTime(2000, 2, 6), PlayerPosition.Midfielder, chelsea) { PlayerId = 11, Appearances = 29, ClubId = 3 },
    new Player("Nicolas Jackson", 15, new DateTime(2001, 6, 5), PlayerPosition.Forward, chelsea) { PlayerId = 12, Appearances = 27, ClubId = 3 }
};
chelsea.Players = chelsea_Players;

premierLeague.Clubs = new List<Club> { manchesterCity, liverpool, chelsea };

// ===== LA LIGA - CLUBS AND PLAYERS =====
var realMadrid = new Club("Real Madrid", "Madrid", 1902, "Santiago Bernabéu", laLiga)
{
    ClubId = 4,
    Points = 90,
    LeagueId = 2
};

var realMadrid_Players = new List<Player>
{
    new Player("Andriy Lunin", 1, new DateTime(1999, 2, 11), PlayerPosition.Goalkeeper, realMadrid) { PlayerId = 13, Appearances = 38, ClubId = 4 },
    new Player("Dani Carvajal", 2, new DateTime(1992, 1, 11), PlayerPosition.Defender, realMadrid) { PlayerId = 14, Appearances = 36, ClubId = 4 },
    new Player("Toni Kroos", 8, new DateTime(1990, 1, 4), PlayerPosition.Midfielder, realMadrid) { PlayerId = 15, Appearances = 34, ClubId = 4 },
    new Player("Vinícius Júnior", 7, new DateTime(2000, 7, 12), PlayerPosition.Forward, realMadrid) { PlayerId = 16, Appearances = 37, ClubId = 4 }
};
realMadrid.Players = realMadrid_Players;

var barcelona = new Club("FC Barcelona", "Barcelona", 1899, "Camp Nou", laLiga)
{
    ClubId = 5,
    Points = 78,
    LeagueId = 2
};

var barcelona_Players = new List<Player>
{
    new Player("Marc-André ter Stegen", 1, new DateTime(1992, 4, 30), PlayerPosition.Goalkeeper, barcelona) { PlayerId = 17, Appearances = 30, ClubId = 5 },
    new Player("Jules Koundé", 23, new DateTime(1998, 11, 4), PlayerPosition.Defender, barcelona) { PlayerId = 18, Appearances = 28, ClubId = 5 },
    new Player("Sergi Roberto", 20, new DateTime(1992, 7, 7), PlayerPosition.Midfielder, barcelona) { PlayerId = 19, Appearances = 32, ClubId = 5 },
    new Player("Robert Lewandowski", 9, new DateTime(1988, 8, 21), PlayerPosition.Forward, barcelona) { PlayerId = 20, Appearances = 35, ClubId = 5 }
};
barcelona.Players = barcelona_Players;

var atleticoMadrid = new Club("Atlético Madrid", "Madrid", 1903, "Metropolitano", laLiga)
{
    ClubId = 6,
    Points = 81,
    LeagueId = 2
};

var atleticoMadrid_Players = new List<Player>
{
    new Player("Jan Oblak", 1, new DateTime(1993, 1, 7), PlayerPosition.Goalkeeper, atleticoMadrid) { PlayerId = 21, Appearances = 38, ClubId = 6 },
    new Player("Kieran Trippier", 2, new DateTime(1990, 9, 19), PlayerPosition.Defender, atleticoMadrid) { PlayerId = 22, Appearances = 25, ClubId = 6 },
    new Player("Rodrigo De Paul", 5, new DateTime(1996, 3, 24), PlayerPosition.Midfielder, atleticoMadrid) { PlayerId = 23, Appearances = 33, ClubId = 6 },
    new Player("Antoine Griezmann", 7, new DateTime(1991, 3, 21), PlayerPosition.Forward, atleticoMadrid) { PlayerId = 24, Appearances = 36, ClubId = 6 }
};
atleticoMadrid.Players = atleticoMadrid_Players;

laLiga.Clubs = new List<Club> { realMadrid, barcelona, atleticoMadrid };

// ===== BUNDESLIGA - CLUBS AND PLAYERS =====
var bayernMunich = new Club("Bayern Munich", "Munich", 1900, "Allianz Arena", bundesliga)
{
    ClubId = 7,
    Points = 88,
    LeagueId = 3
};

var bayernMunich_Players = new List<Player>
{
    new Player("Manuel Neuer", 1, new DateTime(1986, 3, 27), PlayerPosition.Goalkeeper, bayernMunich) { PlayerId = 25, Appearances = 35, ClubId = 7 },
    new Player("Benjamin Pavard", 5, new DateTime(1996, 3, 5), PlayerPosition.Defender, bayernMunich) { PlayerId = 26, Appearances = 32, ClubId = 7 },
    new Player("Joshua Kimmich", 32, new DateTime(1995, 12, 13), PlayerPosition.Midfielder, bayernMunich) { PlayerId = 27, Appearances = 34, ClubId = 7 },
    new Player("Serge Gnabry", 7, new DateTime(1995, 10, 14), PlayerPosition.Forward, bayernMunich) { PlayerId = 28, Appearances = 28, ClubId = 7 }
};
bayernMunich.Players = bayernMunich_Players;

var borussiaDortmund = new Club("Borussia Dortmund", "Dortmund", 1909, "Signal Iduna Park", bundesliga)
{
    ClubId = 8,
    Points = 75,
    LeagueId = 3
};

var borussiaDortmund_Players = new List<Player>
{
    new Player("Gregor Kobel", 1, new DateTime(1997, 6, 24), PlayerPosition.Goalkeeper, borussiaDortmund) { PlayerId = 29, Appearances = 30, ClubId = 8 },
    new Player("Mats Hummels", 15, new DateTime(1988, 12, 16), PlayerPosition.Defender, borussiaDortmund) { PlayerId = 30, Appearances = 28, ClubId = 8 },
    new Player("Marco Reus", 11, new DateTime(1990, 5, 31), PlayerPosition.Midfielder, borussiaDortmund) { PlayerId = 31, Appearances = 25, ClubId = 8 },
    new Player("Sébastien Haller", 9, new DateTime(1994, 6, 22), PlayerPosition.Forward, borussiaDortmund) { PlayerId = 32, Appearances = 26, ClubId = 8 }
};
borussiaDortmund.Players = borussiaDortmund_Players;

var bayerLeverkusen = new Club("Bayer Leverkusen", "Leverkusen", 1904, "BayArena", bundesliga)
{
    ClubId = 9,
    Points = 79,
    LeagueId = 3
};

var bayerLeverkusen_Players = new List<Player>
{
    new Player("Matěj Kovář", 1, new DateTime(1999, 9, 8), PlayerPosition.Goalkeeper, bayerLeverkusen) { PlayerId = 33, Appearances = 34, ClubId = 9 },
    new Player("Jonathan Tah", 14, new DateTime(1996, 2, 11), PlayerPosition.Defender, bayerLeverkusen) { PlayerId = 34, Appearances = 31, ClubId = 9 },
    new Player("Charles Aránguiz", 17, new DateTime(1989, 4, 17), PlayerPosition.Midfielder, bayerLeverkusen) { PlayerId = 35, Appearances = 29, ClubId = 9 },
    new Player("Florian Wirtz", 28, new DateTime(2002, 5, 3), PlayerPosition.Forward, bayerLeverkusen) { PlayerId = 36, Appearances = 33, ClubId = 9 }
};
bayerLeverkusen.Players = bayerLeverkusen_Players;

bundesliga.Clubs = new List<Club> { bayernMunich, borussiaDortmund, bayerLeverkusen };

// ===== CREATE MATCHES =====
var matches = new List<Match>
{
    new Match(manchesterCity, liverpool, new DateTime(2024, 3, 10, 15, 0, 0), 3, 1, premierLeague) { MatchId = 1, HomeClubId = 1, AwayClubId = 2, LeagueId = 1 },
    new Match(liverpool, chelsea, new DateTime(2024, 3, 17, 16, 30, 0), 2, 2, premierLeague) { MatchId = 2, HomeClubId = 2, AwayClubId = 3, LeagueId = 1 },
    new Match(chelsea, manchesterCity, new DateTime(2024, 3, 24, 14, 0, 0), 1, 2, premierLeague) { MatchId = 3, HomeClubId = 3, AwayClubId = 1, LeagueId = 1 },
    new Match(realMadrid, barcelona, new DateTime(2024, 3, 11, 16, 0, 0), 4, 1, laLiga) { MatchId = 4, HomeClubId = 4, AwayClubId = 5, LeagueId = 2 },
    new Match(barcelona, atleticoMadrid, new DateTime(2024, 3, 18, 15, 15, 0), 2, 0, laLiga) { MatchId = 5, HomeClubId = 5, AwayClubId = 6, LeagueId = 2 },
    new Match(atleticoMadrid, realMadrid, new DateTime(2024, 3, 25, 16, 0, 0), 1, 3, laLiga) { MatchId = 6, HomeClubId = 6, AwayClubId = 4, LeagueId = 2 },
    new Match(bayernMunich, borussiaDortmund, new DateTime(2024, 3, 12, 17, 30, 0), 3, 2, bundesliga) { MatchId = 7, HomeClubId = 7, AwayClubId = 8, LeagueId = 3 },
    new Match(borussiaDortmund, bayerLeverkusen, new DateTime(2024, 3, 19, 15, 0, 0), 1, 1, bundesliga) { MatchId = 8, HomeClubId = 8, AwayClubId = 9, LeagueId = 3 },
    new Match(bayerLeverkusen, bayernMunich, new DateTime(2024, 3, 26, 15, 30, 0), 2, 2, bundesliga) { MatchId = 9, HomeClubId = 9, AwayClubId = 7, LeagueId = 3 }
};

premierLeague.Matches = matches.Where(m => m.LeagueId == 1).ToList();
laLiga.Matches = matches.Where(m => m.LeagueId == 2).ToList();
bundesliga.Matches = matches.Where(m => m.LeagueId == 3).ToList();

// ===== CREATE MATCH RATINGS =====
var matchRatings = new List<MatchRating>
{
    new MatchRating(matches[0], 9, "Spectacular display", "Football Critic", new DateTime(2024, 3, 10, 18, 0, 0)) { MatchRatingId = 1, MatchId = 1 },
    new MatchRating(matches[1], 7, "Entertaining but defensive errors", "Sports Analyst", new DateTime(2024, 3, 17, 18, 0, 0)) { MatchRatingId = 2, MatchId = 2 },
    new MatchRating(matches[2], 8, "High intensity match", "Football Critic", new DateTime(2024, 3, 24, 18, 0, 0)) { MatchRatingId = 3, MatchId = 3 },
    new MatchRating(matches[3], 9, "Classical El Clásico performance", "La Liga Expert", new DateTime(2024, 3, 11, 18, 0, 0)) { MatchRatingId = 4, MatchId = 4 },
    new MatchRating(matches[4], 6, "Weak Barcelona performance", "La Liga Expert", new DateTime(2024, 3, 18, 18, 0, 0)) { MatchRatingId = 5, MatchId = 5 }
};

matches[0].MatchRating = matchRatings[0];
matches[1].MatchRating = matchRatings[1];
matches[2].MatchRating = matchRatings[2];
matches[3].MatchRating = matchRatings[3];
matches[4].MatchRating = matchRatings[4];

// ===== CREATE PLAYER MATCH RATINGS =====
var playerMatchRatings = new List<PlayerMatchRating>
{
    new PlayerMatchRating(manchesterCity_Players[3], matches[0], 9, "Hat-trick, dominant display", new DateTime(2024, 3, 10, 18, 0, 0)) { PlayerMatchRatingId = 1, PlayerId = 4, MatchId = 1 },
    new PlayerMatchRating(manchesterCity_Players[2], matches[0], 8, "Controlled midfield", new DateTime(2024, 3, 10, 18, 0, 0)) { PlayerMatchRatingId = 2, PlayerId = 3, MatchId = 1 },
    new PlayerMatchRating(liverpool_Players[2], matches[0], 6, "Below average contribution", new DateTime(2024, 3, 10, 18, 0, 0)) { PlayerMatchRatingId = 3, PlayerId = 7, MatchId = 1 },
    new PlayerMatchRating(liverpool_Players[2], matches[1], 8, "Two goals, excellent form", new DateTime(2024, 3, 17, 18, 0, 0)) { PlayerMatchRatingId = 4, PlayerId = 7, MatchId = 2 },
    new PlayerMatchRating(chelsea_Players[3], matches[2], 7, "Physical presence, one goal", new DateTime(2024, 3, 24, 18, 0, 0)) { PlayerMatchRatingId = 5, PlayerId = 12, MatchId = 3 },
    new PlayerMatchRating(realMadrid_Players[3], matches[3], 9, "Two goals and two assists", new DateTime(2024, 3, 11, 18, 0, 0)) { PlayerMatchRatingId = 6, PlayerId = 16, MatchId = 4 },
    new PlayerMatchRating(barcelona_Players[3], matches[4], 9, "One goal, clinical finishing", new DateTime(2024, 3, 18, 18, 0, 0)) { PlayerMatchRatingId = 7, PlayerId = 20, MatchId = 5 },
    new PlayerMatchRating(bayernMunich_Players[3], matches[6], 8, "Outstanding game, one goal", new DateTime(2024, 3, 12, 18, 0, 0)) { PlayerMatchRatingId = 8, PlayerId = 28, MatchId = 7 }
};

matches[0].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 1).ToList();
matches[1].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 2).ToList();
matches[2].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 3).ToList();
matches[3].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 4).ToList();
matches[4].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 5).ToList();
matches[6].PlayerRatings = playerMatchRatings.Where(pr => pr.MatchId == 7).ToList();

manchesterCity_Players[3].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 4).ToList();
liverpool_Players[2].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 7).ToList();
chelsea_Players[3].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 12).ToList();
realMadrid_Players[3].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 16).ToList();
barcelona_Players[3].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 20).ToList();
bayernMunich_Players[3].MatchRatings = playerMatchRatings.Where(pr => pr.PlayerId == 28).ToList();

// ===== LINQ QUERIES =====
Console.WriteLine("📊 LINQ QUERIES - DEMONSTRATING DATA ANALYSIS\n");

// Query 1: Get all defenders from a specific league
var defendersInPremier = premierLeague.Clubs
    .SelectMany(c => c.Players)
    .Where(p => p.Position == PlayerPosition.Defender)
    .OrderBy(p => p.Name)
    .ToList();

Console.WriteLine($"✓ Query 1: Defenders in Premier League ({defendersInPremier.Count})");
foreach (var defender in defendersInPremier)
{
    Console.WriteLine($"  - {defender.Name} ({defender.Club.Name}) - Jersey #{defender.JerseyNumber}");
}
Console.WriteLine();

// Query 2: Top 3 forwards by appearances
var topForwards = allLeagues
    .SelectMany(l => l.Clubs)
    .SelectMany(c => c.Players)
    .Where(p => p.Position == PlayerPosition.Forward)
    .OrderByDescending(p => p.Appearances)
    .Take(3)
    .ToList();

Console.WriteLine($"✓ Query 2: Top 3 Forwards by Appearances");
foreach (var forward in topForwards)
{
    Console.WriteLine($"  - {forward.Name} ({forward.Club.Name}): {forward.Appearances} appearances");
}
Console.WriteLine();

// Query 3: Average rating across all matches
var avgMatchRating = matchRatings.Count > 0 ? matchRatings.Average(mr => mr.Rating) : 0;
Console.WriteLine($"✓ Query 3: Average Match Rating: {avgMatchRating:F2}");
Console.WriteLine();

// Query 4: Best performing players (rating >= 8)
var bestPerformers = playerMatchRatings
    .Where(pr => pr.Rating >= 8)
    .OrderByDescending(pr => pr.Rating)
    .Select(pr => new { PlayerName = pr.Player.Name, ClubName = pr.Player.Club.Name, pr.Rating, pr.Performance })
    .ToList();

Console.WriteLine($"✓ Query 4: Best Performing Players (Rating >= 8) ({bestPerformers.Count})");
foreach (var performer in bestPerformers)
{
    Console.WriteLine($"  - {performer.PlayerName} ({performer.ClubName}): {performer.Rating}/10 - {performer.Performance}");
}
Console.WriteLine();

// Query 5: Matches by league with their scores
var matchesByLeague = allLeagues
    .Select(l => new
    {
        LeagueName = l.Name,
        MatchCount = l.Matches.Count,
        Matches = l.Matches.Select(m => $"{m.HomeClub.Name} {m.HomeScore}:{m.AwayScore} {m.AwayClub.Name}").ToList()
    })
    .ToList();

Console.WriteLine($"✓ Query 5: Matches by League");
foreach (var league in matchesByLeague)
{
    Console.WriteLine($"  {league.LeagueName} ({league.MatchCount} matches):");
    foreach (var match in league.Matches)
    {
        Console.WriteLine($"    • {match}");
    }
}
Console.WriteLine();

// Query 6: Clubs with most points
var clubsByPoints = allLeagues
    .SelectMany(l => l.Clubs)
    .OrderByDescending(c => c.Points)
    .Take(5)
    .Select(c => new { ClubName = c.Name, c.Points, LeagueName = c.League.Name })
    .ToList();

Console.WriteLine($"✓ Query 6: Top 5 Clubs by Points");
foreach (var club in clubsByPoints)
{
    Console.WriteLine($"  - {club.ClubName} ({club.LeagueName}): {club.Points} pts");
}
Console.WriteLine();

// Query 7: Single player with highest rating
var bestPlayer = playerMatchRatings.OrderByDescending(pr => pr.Rating).FirstOrDefault();
Console.WriteLine($"✓ Query 7: Single Best Rated Player");
if (bestPlayer != null)
{
    Console.WriteLine($"  - {bestPlayer.Player.Name}: {bestPlayer.Rating}/10\n");
}

// ===== ASYNC DEMONSTRATION =====
Console.WriteLine("⏳ ASYNC DEMONSTRATION\n");
await DemoAsyncDataProcessing();
Console.WriteLine("\nProgram completed!");

// Async method to demonstrate async-await concept
async Task DemoAsyncDataProcessing()
{
    Console.WriteLine("Starting async data processing...");
    
    // Simulate fetching data from multiple sources asynchronously
    Task<int> task1 = FetchLeaguesAsync(1500);
    Task<int> task2 = FetchMatchDataAsync(2000);
    Task<int> task3 = FetchPlayerRatingsAsync(1000);

    Console.WriteLine("Tasks started, processing in background...");

    int leaguesCount = await task1;
    int matchesCount = await task2;
    int ratingsCount = await task3;

    Console.WriteLine($"\n✓ Data Processing Complete:");
    Console.WriteLine($"  - Leagues loaded: {leaguesCount}");
    Console.WriteLine($"  - Matches loaded: {matchesCount}");
    Console.WriteLine($"  - Ratings loaded: {ratingsCount}");
}

async Task<int> FetchLeaguesAsync(int delayMs)
{
    await Task.Delay(delayMs);
    Console.WriteLine($"✓ Leagues fetched ({delayMs}ms)");
    return allLeagues.Count;
}

async Task<int> FetchMatchDataAsync(int delayMs)
{
    await Task.Delay(delayMs);
    Console.WriteLine($"✓ Matches fetched ({delayMs}ms)");
    return matches.Count;
}

async Task<int> FetchPlayerRatingsAsync(int delayMs)
{
    await Task.Delay(delayMs);
    Console.WriteLine($"✓ Player ratings fetched ({delayMs}ms)");
    return playerMatchRatings.Count;
}
