# Semantic DB Model

## Purpose

This document summarizes the semantic database model used by the ProbaMala ASP.NET Core MVC application. It lists the main entities, their primary properties, and the relationships between them.

## Entity Overview

### League

Represents a football competition such as Premier League or La Liga.

Main properties:
- `Id` - primary key
- `Name` - league name

Navigation properties:
- `Clubs` - clubs that belong to the league
- `Matches` - matches played in the league

### Club

Represents a football club.

Main properties:
- `Id` - primary key
- `Name` - club name
- `FoundedDate` - foundation date
- `LeagueId` - foreign key to League

Navigation properties:
- `League` - parent league
- `Players` - players currently assigned to the club
- `HomeMatches` - matches where the club is the home team
- `AwayMatches` - matches where the club is the away team

### Player

Represents an individual football player.

Main properties:
- `Id` - primary key
- `FirstName` - player's first name
- `LastName` - player's last name
- `DateOfBirth` - player's date of birth
- `Position` - enum value: `Goalkeeper`, `Defender`, `Midfielder`, `Forward`
- `ClubId` - foreign key to Club
- `Nationality` - player's nationality

Navigation properties:
- `Club` - club the player belongs to
- `Ratings` - ratings given to the player in specific matches

### Match

Represents a football match between two clubs.

Main properties:
- `Id` - primary key
- `LeagueId` - foreign key to League
- `HomeTeamId` - foreign key to Club, home team
- `AwayTeamId` - foreign key to Club, away team
- `Date` - date of the match
- `HomeGoals` - number of goals scored by the home team
- `AwayGoals` - number of goals scored by the away team

Navigation properties:
- `League` - league in which the match was played
- `HomeTeam` - club playing at home
- `AwayTeam` - club playing away
- `Ratings` - player ratings associated with the match

### Rating

Represents a user-submitted score and optional comment for a player's performance in a match.

Main properties:
- `Id` - primary key
- `PlayerId` - foreign key to Player
- `MatchId` - foreign key to Match
- `UserId` - foreign key to User
- `Score` - integer score from 1 to 10
- `Comment` - optional textual comment

Navigation properties:
- `Player` - rated player
- `Match` - match for which the rating was submitted
- `User` - user who submitted the rating

### User

Represents an application user who can submit ratings.

Main properties:
- `Id` - primary key
- `FirstName` - user's first name
- `LastName` - user's last name
- `Email` - user's email address

Navigation properties:
- `Ratings` - ratings created by the user

## Relationship Summary

- `League` 1-N `Club`
One league can contain many clubs. Each club belongs to exactly one league.

- `League` 1-N `Match`
One league can contain many matches. Each match belongs to exactly one league.

- `Club` 1-N `Player`
One club can have many players. Each player belongs to exactly one club.

- `Club` 1-N `Match` as home team
One club can appear in many matches as `HomeTeam`.

- `Club` 1-N `Match` as away team
One club can appear in many matches as `AwayTeam`.

- `Player` 1-N `Rating`
One player can have many ratings. Each rating belongs to exactly one player.

- `Match` 1-N `Rating`
One match can have many ratings. Each rating belongs to exactly one match.

- `User` 1-N `Rating`
One user can create many ratings. Each rating belongs to exactly one user.

## Foreign Key Overview

| Entity | Foreign key | References |
| --- | --- | --- |
| `Club` | `LeagueId` | `League.Id` |
| `Player` | `ClubId` | `Club.Id` |
| `Match` | `LeagueId` | `League.Id` |
| `Match` | `HomeTeamId` | `Club.Id` |
| `Match` | `AwayTeamId` | `Club.Id` |
| `Rating` | `PlayerId` | `Player.Id` |
| `Rating` | `MatchId` | `Match.Id` |
| `Rating` | `UserId` | `User.Id` |

## Notes

- The model uses EF Core annotations such as `Key`, `Required`, `MaxLength`, `ForeignKey`, and `InverseProperty`.
- `Position` is an enum in the domain model, not a separate entity table.
- `Match` contains two separate references to `Club`, so home and away relationships are explicitly modeled.