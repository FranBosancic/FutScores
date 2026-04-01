using Proba.Models;

// ===== INITIALIZING DATA =====
Console.WriteLine("=== FutScores - Football Match and Player Performance Rating Platform ===\n");

// Create Leagues
var premierLeague = new League
{
    LeagueId = 1,
    Name = "Premier League",
    Country = "England",
    FoundationYear = 1992,
    TotalClubs = 20
};

var laLiga = new League
{
    LeagueId = 2,
    Name = "La Liga",
    Country = "Spain",
    FoundationYear = 1929,
    TotalClubs = 20
};

var bundesliga = new League
{
    LeagueId = 3,
    Name = "Bundesliga",
    Country = "Germany",
    FoundationYear = 1963,
    TotalClubs = 18
};

var allLeagues = new List<League> { premierLeague, laLiga, bundesliga };

// ===== PREMIER LEAGUE - CLUBS AND PLAYERS =====
var manchesterCity = new Club
{
    ClubId = 1,
    Name = "Manchester City",
    City = "Manchester",
    FoundationYear = 1880,
    Stadium = "Etihad Stadium",
    Points = 89,
    LeagueId = 1,
    League = premierLeague
};

var manchesterCity_Players = new List<Player>
{
    new Player { PlayerId = 1, Name = "Ederson", JerseyNumber = 1, DateOfBirth = new DateTime(1993, 8, 17), Position = PlayerPosition.Goalkeeper, Appearances = 38, ClubId = 1, Club = manchesterCity },
    new Player { PlayerId = 2, Name = "Kyle Walker", JerseyNumber = 2, DateOfBirth = new DateTime(1990, 5, 28), Position = PlayerPosition.Defender, Appearances = 35, ClubId = 1, Club = manchesterCity },
    new Player { PlayerId = 3, Name = "Rodri", JerseyNumber = 16, DateOfBirth = new DateTime(1996, 6, 22), Position = PlayerPosition.Midfielder, Appearances = 37, ClubId = 1, Club = manchesterCity },
    new Player { PlayerId = 4, Name = "Erling Haaland", JerseyNumber = 9, DateOfBirth = new DateTime(2000, 7, 21), Position = PlayerPosition.Forward, Appearances = 36, ClubId = 1, Club = manchesterCity }
};
manchesterCity.Players = manchesterCity_Players;

var liverpool = new Club
{
    ClubId = 2,
    Name = "Liverpool",
    City = "Liverpool",
    FoundationYear = 1892,
    Stadium = "Anfield",
    Points = 82,
    LeagueId = 1,
    League = premierLeague
};

var liverpool_Players = new List<Player>
{
    new Player { PlayerId = 5, Name = "Alisson", JerseyNumber = 1, DateOfBirth = new DateTime(1992, 10, 2), Position = PlayerPosition.Goalkeeper, Appearances = 38, ClubId = 2, Club = liverpool },
    new Player { PlayerId = 6, Name = "Trent Alexander-Arnold", JerseyNumber = 66, DateOfBirth = new DateTime(1998, 10, 7), Position = PlayerPosition.Defender, Appearances = 32, ClubId = 2, Club = liverpool },
    new Player { PlayerId = 7, Name = "Mohamed Salah", JerseyNumber = 11, DateOfBirth = new DateTime(1992, 6, 15), Position = PlayerPosition.Forward, Appearances = 37, ClubId = 2, Club = liverpool },
    new Player { PlayerId = 8, Name = "Alexis Mac Allister", JerseyNumber = 10, DateOfBirth = new DateTime(1998, 7, 24), Position = PlayerPosition.Midfielder, Appearances = 28, ClubId = 2, Club = liverpool }
};
liverpool.Players = liverpool_Players;

var chelsea = new Club
{
    ClubId = 3,
    Name = "Chelsea",
    City = "London",
    FoundationYear = 1905,
    Stadium = "Stamford Bridge",
    Points = 76,
    LeagueId = 1,
    League = premierLeague
};

var chelsea_Players = new List<Player>
{
    new Player { PlayerId = 9, Name = "Robert Sánchez", JerseyNumber = 1, DateOfBirth = new DateTime(1996, 11, 18), Position = PlayerPosition.Goalkeeper, Appearances = 30, ClubId = 3, Club = chelsea },
    new Player { PlayerId = 10, Name = "Reece James", JerseyNumber = 24, DateOfBirth = new DateTime(1999, 12, 8), Position = PlayerPosition.Defender, Appearances = 25, ClubId = 3, Club = chelsea },
    new Player { PlayerId = 11, Name = "Conor Gallagher", JerseyNumber = 23, DateOfBirth = new DateTime(2000, 2, 6), Position = PlayerPosition.Midfielder, Appearances = 29, ClubId = 3, Club = chelsea },
    new Player { PlayerId = 12, Name = "Nicolas Jackson", JerseyNumber = 15, DateOfBirth = new DateTime(2001, 6, 5), Position = PlayerPosition.Forward, Appearances = 27, ClubId = 3, Club = chelsea }
};
chelsea.Players = chelsea_Players;

premierLeague.Clubs = new List<Club> { manchesterCity, liverpool, chelsea };

// ===== LA LIGA - CLUBS AND PLAYERS =====
var realMadrid = new Club
{
    ClubId = 4,
    Name = "Real Madrid",
    City = "Madrid",
    FoundationYear = 1902,
    Stadium = "Santiago Bernabéu",
    Points = 90,
    LeagueId = 2,
    League = laLiga
};

var realMadrid_Players = new List<Player>
{
    new Player { PlayerId = 13, Name = "Andriy Lunin", JerseyNumber = 1, DateOfBirth = new DateTime(1999, 2, 11), Position = PlayerPosition.Goalkeeper, Appearances = 38, ClubId = 4, Club = realMadrid },
    new Player { PlayerId = 14, Name = "Dani Carvajal", JerseyNumber = 2, DateOfBirth = new DateTime(1992, 1, 11), Position = PlayerPosition.Defender, Appearances = 36, ClubId = 4, Club = realMadrid },
    new Player { PlayerId = 15, Name = "Toni Kroos", JerseyNumber = 8, DateOfBirth = new DateTime(1990, 1, 4), Position = PlayerPosition.Midfielder, Appearances = 34, ClubId = 4, Club = realMadrid },
    new Player { PlayerId = 16, Name = "Vinícius Júnior", JerseyNumber = 7, DateOfBirth = new DateTime(2000, 7, 12), Position = PlayerPosition.Forward, Appearances = 37, ClubId = 4, Club = realMadrid }
};
realMadrid.Players = realMadrid_Players;

var barcelona = new Club
{
    ClubId = 5,
    Name = "FC Barcelona",
    City = "Barcelona",
    FoundationYear = 1899,
    Stadium = "Camp Nou",
    Points = 78,
    LeagueId = 2,
    League = laLiga
};

var barcelona_Players = new List<Player>
{
    new Player { PlayerId = 17, Name = "Marc-André ter Stegen", JerseyNumber = 1, DateOfBirth = new DateTime(1992, 4, 30), Position = PlayerPosition.Goalkeeper, Appearances = 30, ClubId = 5, Club = barcelona },
    new Player { PlayerId = 18, Name = "Jules Koundé", JerseyNumber = 23, DateOfBirth = new DateTime(1998, 11, 4), Position = PlayerPosition.Defender, Appearances = 28, ClubId = 5, Club = barcelona },
    new Player { PlayerId = 19, Name = "Sergi Roberto", JerseyNumber = 20, DateOfBirth = new DateTime(1992, 7, 7), Position = PlayerPosition.Midfielder, Appearances = 32, ClubId = 5, Club = barcelona },
    new Player { PlayerId = 20, Name = "Robert Lewandowski", JerseyNumber = 9, DateOfBirth = new DateTime(1988, 8, 21), Position = PlayerPosition.Forward, Appearances = 35, ClubId = 5, Club = barcelona }
};
barcelona.Players = barcelona_Players;

var atleticoMadrid = new Club
{
    ClubId = 6,
    Name = "Atlético Madrid",
    City = "Madrid",
    FoundationYear = 1903,
    Stadium = "Metropolitano",
    Points = 81,
    LeagueId = 2,
    League = laLiga
};

var atleticoMadrid_Players = new List<Player>
{
    new Player { PlayerId = 21, Name = "Jan Oblak", JerseyNumber = 1, DateOfBirth = new DateTime(1993, 1, 7), Position = PlayerPosition.Goalkeeper, Appearances = 38, ClubId = 6, Club = atleticoMadrid },
    new Player { PlayerId = 22, Name = "Kieran Trippier", JerseyNumber = 2, DateOfBirth = new DateTime(1990, 9, 19), Position = PlayerPosition.Defender, Appearances = 25, ClubId = 6, Club = atleticoMadrid },
    new Player { PlayerId = 23, Name = "Rodrigo De Paul", JerseyNumber = 5, DateOfBirth = new DateTime(1996, 3, 24), Position = PlayerPosition.Midfielder, Appearances = 33, ClubId = 6, Club = atleticoMadrid },
    new Player { PlayerId = 24, Name = "Antoine Griezmann", JerseyNumber = 7, DateOfBirth = new DateTime(1991, 3, 21), Position = PlayerPosition.Forward, Appearances = 36, ClubId = 6, Club = atleticoMadrid }
};
atleticoMadrid.Players = atleticoMadrid_Players;

laLiga.Clubs = new List<Club> { realMadrid, barcelona, atleticoMadrid };

// ===== BUNDESLIGA - CLUBS AND PLAYERS =====
var bayernMunich = new Club
{
    ClubId = 7,
    Name = "Bayern Munich",
    City = "Munich",
    FoundationYear = 1900,
    Stadium = "Allianz Arena",
    Points = 88,
    LeagueId = 3,
    League = bundesliga
};

var bayernMunich_Players = new List<Player>
{
    new Player { PlayerId = 25, Name = "Manuel Neuer", JerseyNumber = 1, DateOfBirth = new DateTime(1986, 3, 27), Position = PlayerPosition.Goalkeeper, Appearances = 35, ClubId = 7, Club = bayernMunich },
    new Player { PlayerId = 26, Name = "Benjamin Pavard", JerseyNumber = 5, DateOfBirth = new DateTime(1996, 3, 5), Position = PlayerPosition.Defender, Appearances = 32, ClubId = 7, Club = bayernMunich },
    new Player { PlayerId = 27, Name = "Joshua Kimmich", JerseyNumber = 32, DateOfBirth = new DateTime(1995, 12, 13), Position = PlayerPosition.Midfielder, Appearances = 34, ClubId = 7, Club = bayernMunich },
    new Player { PlayerId = 28, Name = "Serge Gnabry", JerseyNumber = 7, DateOfBirth = new DateTime(1995, 10, 14), Position = PlayerPosition.Forward, Appearances = 28, ClubId = 7, Club = bayernMunich }
};
bayernMunich.Players = bayernMunich_Players;

var borussiaDortmund = new Club
{
    ClubId = 8,
    Name = "Borussia Dortmund",
    City = "Dortmund",
    FoundationYear = 1909,
    Stadium = "Signal Iduna Park",
    Points = 75,
    LeagueId = 3,
    League = bundesliga
};

var borussiaDortmund_Players = new List<Player>
{
    new Player { PlayerId = 29, Name = "Gregor Kobel", JerseyNumber = 1, DateOfBirth = new DateTime(1997, 6, 24), Position = PlayerPosition.Goalkeeper, Appearances = 30, ClubId = 8, Club = borussiaDortmund },
    new Player { PlayerId = 30, Name = "Mats Hummels", JerseyNumber = 15, DateOfBirth = new DateTime(1988, 12, 16), Position = PlayerPosition.Defender, Appearances = 28, ClubId = 8, Club = borussiaDortmund },
    new Player { PlayerId = 31, Name = "Marco Reus", JerseyNumber = 11, DateOfBirth = new DateTime(1990, 5, 31), Position = PlayerPosition.Midfielder, Appearances = 25, ClubId = 8, Club = borussiaDortmund },
    new Player { PlayerId = 32, Name = "Sébastien Haller", JerseyNumber = 9, DateOfBirth = new DateTime(1994, 6, 22), Position = PlayerPosition.Forward, Appearances = 26, ClubId = 8, Club = borussiaDortmund }
};
borussiaDortmund.Players = borussiaDortmund_Players;

var bayerLeverkusen = new Club
{
    ClubId = 9,
    Name = "Bayer Leverkusen",
    City = "Leverkusen",
    FoundationYear = 1904,
    Stadium = "BayArena",
    Points = 79,
    LeagueId = 3,
    League = bundesliga
};

var bayerLeverkusen_Players = new List<Player>
{
    new Player { PlayerId = 33, Name = "Matěj Kovář", JerseyNumber = 1, DateOfBirth = new DateTime(1999, 9, 8), Position = PlayerPosition.Goalkeeper, Appearances = 34, ClubId = 9, Club = bayerLeverkusen },
    new Player { PlayerId = 34, Name = "Jonathan Tah", JerseyNumber = 14, DateOfBirth = new DateTime(1996, 2, 11), Position = PlayerPosition.Defender, Appearances = 31, ClubId = 9, Club = bayerLeverkusen },
    new Player { PlayerId = 35, Name = "Charles Aránguiz", JerseyNumber = 17, DateOfBirth = new DateTime(1989, 4, 17), Position = PlayerPosition.Midfielder, Appearances = 29, ClubId = 9, Club = bayerLeverkusen },
    new Player { PlayerId = 36, Name = "Florian Wirtz", JerseyNumber = 28, DateOfBirth = new DateTime(2002, 5, 3), Position = PlayerPosition.Forward, Appearances = 33, ClubId = 9, Club = bayerLeverkusen }
};
bayerLeverkusen.Players = bayerLeverkusen_Players;

bundesliga.Clubs = new List<Club> { bayernMunich, borussiaDortmund, bayerLeverkusen };

// ===== CREATE MATCHES =====
var matches = new List<Match>
{
    new Match { MatchId = 1, HomeClubId = 1, HomeClub = manchesterCity, AwayClubId = 2, AwayClub = liverpool, MatchDate = new DateTime(2024, 3, 10, 15, 0, 0), HomeScore = 3, AwayScore = 1, LeagueId = 1, League = premierLeague },
    new Match { MatchId = 2, HomeClubId = 2, HomeClub = liverpool, AwayClubId = 3, AwayClub = chelsea, MatchDate = new DateTime(2024, 3, 17, 16, 30, 0), HomeScore = 2, AwayScore = 2, LeagueId = 1, League = premierLeague },
    new Match { MatchId = 3, HomeClubId = 3, HomeClub = chelsea, AwayClubId = 1, AwayClub = manchesterCity, MatchDate = new DateTime(2024, 3, 24, 14, 0, 0), HomeScore = 1, AwayScore = 2, LeagueId = 1, League = premierLeague },
    new Match { MatchId = 4, HomeClubId = 4, HomeClub = realMadrid, AwayClubId = 5, AwayClub = barcelona, MatchDate = new DateTime(2024, 3, 11, 16, 0, 0), HomeScore = 4, AwayScore = 1, LeagueId = 2, League = laLiga },
    new Match { MatchId = 5, HomeClubId = 5, HomeClub = barcelona, AwayClubId = 6, AwayClub = atleticoMadrid, MatchDate = new DateTime(2024, 3, 18, 15, 15, 0), HomeScore = 2, AwayScore = 0, LeagueId = 2, League = laLiga },
    new Match { MatchId = 6, HomeClubId = 6, HomeClub = atleticoMadrid, AwayClubId = 4, AwayClub = realMadrid, MatchDate = new DateTime(2024, 3, 25, 16, 0, 0), HomeScore = 1, AwayScore = 3, LeagueId = 2, League = laLiga },
    new Match { MatchId = 7, HomeClubId = 7, HomeClub = bayernMunich, AwayClubId = 8, AwayClub = borussiaDortmund, MatchDate = new DateTime(2024, 3, 12, 17, 30, 0), HomeScore = 3, AwayScore = 2, LeagueId = 3, League = bundesliga },
    new Match { MatchId = 8, HomeClubId = 8, HomeClub = borussiaDortmund, AwayClubId = 9, AwayClub = bayerLeverkusen, MatchDate = new DateTime(2024, 3, 19, 15, 0, 0), HomeScore = 1, AwayScore = 1, LeagueId = 3, League = bundesliga },
    new Match { MatchId = 9, HomeClubId = 9, HomeClub = bayerLeverkusen, AwayClubId = 7, AwayClub = bayernMunich, MatchDate = new DateTime(2024, 3, 26, 15, 30, 0), HomeScore = 2, AwayScore = 2, LeagueId = 3, League = bundesliga }
};

premierLeague.Matches = matches.Where(m => m.LeagueId == 1).ToList();
laLiga.Matches = matches.Where(m => m.LeagueId == 2).ToList();
bundesliga.Matches = matches.Where(m => m.LeagueId == 3).ToList();

// ===== CREATE MATCH RATINGS =====
var matchRatings = new List<MatchRating>
{
    new MatchRating { MatchRatingId = 1, MatchId = 1, Match = matches[0], Rating = 9, Comments = "Spectacular display", RatedAt = new DateTime(2024, 3, 10, 18, 0, 0), RatedBy = "Football Critic" },
    new MatchRating { MatchRatingId = 2, MatchId = 2, Match = matches[1], Rating = 7, Comments = "Entertaining but defensive errors", RatedAt = new DateTime(2024, 3, 17, 18, 0, 0), RatedBy = "Sports Analyst" },
    new MatchRating { MatchRatingId = 3, MatchId = 3, Match = matches[2], Rating = 8, Comments = "High intensity match", RatedAt = new DateTime(2024, 3, 24, 18, 0, 0), RatedBy = "Football Critic" },
    new MatchRating { MatchRatingId = 4, MatchId = 4, Match = matches[3], Rating = 9, Comments = "Classical El Clásico performance", RatedAt = new DateTime(2024, 3, 11, 18, 0, 0), RatedBy = "La Liga Expert" },
    new MatchRating { MatchRatingId = 5, MatchId = 5, Match = matches[4], Rating = 6, Comments = "Weak Barcelona performance", RatedAt = new DateTime(2024, 3, 18, 18, 0, 0), RatedBy = "La Liga Expert" }
};

matches[0].MatchRating = matchRatings[0];
matches[1].MatchRating = matchRatings[1];
matches[2].MatchRating = matchRatings[2];
matches[3].MatchRating = matchRatings[3];
matches[4].MatchRating = matchRatings[4];

// ===== CREATE PLAYER MATCH RATINGS =====
var playerMatchRatings = new List<PlayerMatchRating>
{
    new PlayerMatchRating { PlayerMatchRatingId = 1, PlayerId = 4, Player = manchesterCity_Players[3], MatchId = 1, Match = matches[0], Rating = 9, Performance = "Hat-trick, dominant display", RatedAt = new DateTime(2024, 3, 10, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 2, PlayerId = 3, Player = manchesterCity_Players[2], MatchId = 1, Match = matches[0], Rating = 8, Performance = "Controlled midfield", RatedAt = new DateTime(2024, 3, 10, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 3, PlayerId = 7, Player = liverpool_Players[2], MatchId = 1, Match = matches[0], Rating = 6, Performance = "Below average contribution", RatedAt = new DateTime(2024, 3, 10, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 4, PlayerId = 7, Player = liverpool_Players[2], MatchId = 2, Match = matches[1], Rating = 8, Performance = "Two goals, excellent form", RatedAt = new DateTime(2024, 3, 17, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 5, PlayerId = 12, Player = chelsea_Players[3], MatchId = 3, Match = matches[2], Rating = 7, Performance = "Physical presence, one goal", RatedAt = new DateTime(2024, 3, 24, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 6, PlayerId = 16, Player = realMadrid_Players[3], MatchId = 4, Match = matches[3], Rating = 9, Performance = "Two goals and two assists", RatedAt = new DateTime(2024, 3, 11, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 7, PlayerId = 20, Player = barcelona_Players[3], MatchId = 5, Match = matches[4], Rating = 9, Performance = "One goal, clinical finishing", RatedAt = new DateTime(2024, 3, 18, 18, 0, 0) },
    new PlayerMatchRating { PlayerMatchRatingId = 8, PlayerId = 28, Player = bayernMunich_Players[3], MatchId = 7, Match = matches[6], Rating = 8, Performance = "Outstanding game, one goal", RatedAt = new DateTime(2024, 3, 12, 18, 0, 0) }
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
