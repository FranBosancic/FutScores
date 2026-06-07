using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IMatchRepository
    {
        /// <summary>Returns matches optionally filtered by text search and/or league.</summary>
        List<MatchDetailsViewModel> GetAll(string? query = null, int? leagueId = null);
        MatchDetailsViewModel? GetById(int id);
        MatchFormViewModel BuildFormModel();
        MatchFormViewModel? GetFormById(int id);
        void PopulateFormOptions(MatchFormViewModel model);
        List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null);
        bool LeagueExists(int leagueId);
        bool ClubExists(int clubId);
        bool ClubBelongsToLeague(int clubId, int leagueId);
        int Add(MatchFormViewModel model);
        bool Update(int id, MatchFormViewModel model);
        bool Delete(int id);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _dbContext;

        public MatchRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MatchDetailsViewModel> GetAll(string? query = null, int? leagueId = null)
        {
            var matchesQuery = _dbContext.Matches
                .AsNoTracking()
                .Include(match => match.League)
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .AsQueryable();

            // Filter to a specific league when coming from a league nav dropdown
            if (leagueId.HasValue)
            {
                matchesQuery = matchesQuery.Where(match => match.LeagueId == leagueId.Value);
            }

            // Full-text search across league name, team names, and goal counts
            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                var parsedGoals = int.TryParse(normalizedQuery, out var goals);

                matchesQuery = matchesQuery.Where(match =>
                    match.League.Name.ToLower().Contains(normalizedQuery) ||
                    match.HomeTeam.Name.ToLower().Contains(normalizedQuery) ||
                    match.AwayTeam.Name.ToLower().Contains(normalizedQuery) ||
                    (parsedGoals && (match.HomeGoals == goals || match.AwayGoals == goals)));
            }

            return matchesQuery
                .OrderByDescending(match => match.Date)
                // Count ratings in SQL (the Ratings nav isn't included, so counting it
                // post-materialization would always yield 0 without lazy loading).
                .Select(match => new { Match = match, RatingCount = match.Ratings.Count })
                .AsEnumerable()
                .Select((row, index) =>
                {
                    var match = row.Match;
                    var statusLabel = index == 0 ? "Featured" : index < 4 ? "Final" : "Recent";
                    var statusTone = index == 0 ? "live" : index < 4 ? "final" : "recent";

                    return new MatchDetailsViewModel
                    {
                        Id = match.Id,
                        LeagueId = match.LeagueId,
                        HomeTeamId = match.HomeTeamId,
                        AwayTeamId = match.AwayTeamId,
                        Date = match.Date,
                        KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                        StatusLabel = statusLabel,
                        StatusTone = statusTone,
                        LeagueName = match.League.Name,
                        HomeTeamName = match.HomeTeam.Name,
                        AwayTeamName = match.AwayTeam.Name,
                        HomeGoals = match.HomeGoals,
                        AwayGoals = match.AwayGoals,
                        RatingCount = row.RatingCount
                    };
                })
                .ToList();
        }

        public MatchDetailsViewModel? GetById(int id)
        {
            var match = _dbContext.Matches
                .AsNoTracking()
                .Include(m => m.League)
                .Include(m => m.HomeTeam).ThenInclude(club => club.Players)
                .Include(m => m.AwayTeam).ThenInclude(club => club.Players)
                .Include(m => m.Ratings).ThenInclude(rating => rating.Player)
                .Include(m => m.Ratings).ThenInclude(rating => rating.User)
                .FirstOrDefault(m => m.Id == id);

            if (match == null)
            {
                return null;
            }

            // Per-player rating aggregates within this match.
            var statsByPlayer = match.Ratings
                .GroupBy(rating => rating.PlayerId)
                .ToDictionary(
                    group => group.Key,
                    group => new { Count = group.Count(), Average = group.Average(rating => rating.Score) });

            // Player of the match: best average among rated players (count breaks ties).
            var topPlayerId = statsByPlayer.Count > 0
                ? statsByPlayer
                    .OrderByDescending(entry => entry.Value.Average)
                    .ThenByDescending(entry => entry.Value.Count)
                    .First().Key
                : (int?)null;

            var topPlayer = topPlayerId.HasValue
                ? match.Ratings.First(rating => rating.PlayerId == topPlayerId.Value).Player
                : null;

            MatchSquadPlayerViewModel ToSquadPlayer(Player player)
            {
                statsByPlayer.TryGetValue(player.Id, out var stat);
                return new MatchSquadPlayerViewModel
                {
                    Id = player.Id,
                    FullName = $"{player.FirstName} {player.LastName}",
                    Position = player.Position,
                    RatingCount = stat?.Count ?? 0,
                    AverageScore = stat?.Average,
                    IsTopRated = topPlayerId == player.Id
                };
            }

            List<MatchSquadPlayerViewModel> BuildSquad(IEnumerable<Player> players) => players
                .Select(ToSquadPlayer)
                .OrderBy(player => player.Position)
                .ThenBy(player => player.FullName)
                .ToList();

            double? AverageOf(IEnumerable<Rating> ratings)
            {
                var list = ratings.ToList();
                return list.Count > 0 ? list.Average(rating => rating.Score) : (double?)null;
            }

            return new MatchDetailsViewModel
            {
                Id = match.Id,
                LeagueId = match.LeagueId,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                Date = match.Date,
                KickoffLabel = match.Date.ToString("MMM dd, yyyy"),
                StatusLabel = "Final",
                StatusTone = "final",
                LeagueName = match.League.Name,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeGoals = match.HomeGoals,
                AwayGoals = match.AwayGoals,
                RatingCount = match.Ratings.Count,
                AverageRating = AverageOf(match.Ratings),
                HomeAverageRating = AverageOf(match.Ratings.Where(rating => rating.Player.ClubId == match.HomeTeamId)),
                AwayAverageRating = AverageOf(match.Ratings.Where(rating => rating.Player.ClubId == match.AwayTeamId)),
                TopRatedPlayerName = topPlayer != null ? $"{topPlayer.FirstName} {topPlayer.LastName}" : null,
                TopRatedPlayerScore = topPlayerId.HasValue ? statsByPlayer[topPlayerId.Value].Average : null,
                HomeSquad = BuildSquad(match.HomeTeam.Players),
                AwaySquad = BuildSquad(match.AwayTeam.Players),
                Ratings = match.Ratings
                    .OrderByDescending(rating => rating.Score)
                    .Select(rating => new RatingDetailsViewModel
                    {
                        Id = rating.Id,
                        PlayerId = rating.PlayerId,
                        MatchId = rating.MatchId,
                        UserId = rating.UserId,
                        PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                        MatchDescription = $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} on {match.Date:yyyy-MM-dd}",
                        UserName = $"{rating.User.FirstName} {rating.User.LastName}",
                        Score = rating.Score,
                        Comment = rating.Comment,
                        HomeTeamName = match.HomeTeam.Name,
                        AwayTeamName = match.AwayTeam.Name,
                        HomeGoals = match.HomeGoals,
                        AwayGoals = match.AwayGoals
                    })
                    .ToList()
            };
        }

        public MatchFormViewModel BuildFormModel()
        {
            var model = new MatchFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public MatchFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Matches
                .AsNoTracking()
                .Where(match => match.Id == id)
                .Select(match => new MatchFormViewModel
                {
                    Id = match.Id,
                    LeagueId = match.LeagueId,
                    HomeTeamId = match.HomeTeamId,
                    AwayTeamId = match.AwayTeamId,
                    Date = match.Date,
                    HomeGoals = match.HomeGoals,
                    AwayGoals = match.AwayGoals
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(MatchFormViewModel model)
        {
            model.LeagueOptions = _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(league => league.Name)
                .Select(league => new SelectListItem
                {
                    Value = league.Id.ToString(),
                    Text = league.Name,
                    Selected = model.LeagueId == league.Id
                })
                .ToList();

            // Only build the club lists once a league is known, so a fresh Create
            // form renders the team selects empty + disabled, while Edit / invalid
            // postback re-renders them fully and pre-selected.
            if (model.LeagueId.HasValue)
            {
                var clubs = GetClubsInLeague(model.LeagueId.Value);
                model.HomeTeamOptions = ToSelectList(clubs, model.HomeTeamId);

                var awayClubs = model.HomeTeamId.HasValue
                    ? clubs.Where(club => club.Id != model.HomeTeamId.Value).ToList()
                    : clubs;
                model.AwayTeamOptions = ToSelectList(awayClubs, model.AwayTeamId);
            }
        }

        public List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Where(club => club.LeagueId == leagueId && (excludeClubId == null || club.Id != excludeClubId))
                .OrderBy(club => club.Name)
                .Select(club => new CascadeOptionViewModel
                {
                    Id = club.Id,
                    Label = club.Name
                })
                .ToList();
        }

        private static List<SelectListItem> ToSelectList(IEnumerable<CascadeOptionViewModel> options, int? selectedId)
        {
            return options
                .Select(option => new SelectListItem
                {
                    Value = option.Id.ToString(),
                    Text = option.Label,
                    Selected = selectedId == option.Id
                })
                .ToList();
        }

        public bool LeagueExists(int leagueId)
        {
            return _dbContext.Leagues.Any(league => league.Id == leagueId);
        }

        public bool ClubExists(int clubId)
        {
            return _dbContext.Clubs.Any(club => club.Id == clubId);
        }

        public bool ClubBelongsToLeague(int clubId, int leagueId)
        {
            return _dbContext.Clubs.Any(club => club.Id == clubId && club.LeagueId == leagueId);
        }

        public int Add(MatchFormViewModel model)
        {
            var entity = new Match
            {
                LeagueId = model.LeagueId!.Value,
                HomeTeamId = model.HomeTeamId!.Value,
                AwayTeamId = model.AwayTeamId!.Value,
                Date = model.Date,
                HomeGoals = model.HomeGoals,
                AwayGoals = model.AwayGoals
            };

            _dbContext.Matches.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, MatchFormViewModel model)
        {
            var entity = _dbContext.Matches.FirstOrDefault(match => match.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.LeagueId = model.LeagueId!.Value;
            entity.HomeTeamId = model.HomeTeamId!.Value;
            entity.AwayTeamId = model.AwayTeamId!.Value;
            entity.Date = model.Date;
            entity.HomeGoals = model.HomeGoals;
            entity.AwayGoals = model.AwayGoals;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Matches.FirstOrDefault(match => match.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Matches.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}