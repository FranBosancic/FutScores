using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IRatingRepository
    {
        List<RatingDetailsViewModel> GetAll(string? query = null);
        RatingDetailsViewModel? GetById(int id);
        RatingFormViewModel BuildFormModel();
        RatingFormViewModel? GetFormById(int id);
        void PopulateFormOptions(RatingFormViewModel model);

        // Cascade data sources (also surfaced as JSON endpoints by the controller).
        List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null);
        List<CascadeOptionViewModel> GetMatchesBetween(int homeTeamId, int awayTeamId);
        List<CascadeOptionViewModel> GetPlayersForMatch(int matchId);

        // Consistency checks for server-side validation.
        bool LeagueExists(int leagueId);
        bool ClubInLeague(int clubId, int leagueId);
        bool MatchHasTeams(int matchId, int leagueId, int homeTeamId, int awayTeamId);
        bool PlayerExists(int playerId);
        bool IsPlayerInMatch(int playerId, int matchId);
        bool UserExists(int userId);

        int Add(RatingFormViewModel model);
        bool Update(int id, RatingFormViewModel model);
        bool Delete(int id);
    }

    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _dbContext;

        public RatingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<RatingDetailsViewModel> GetAll(string? query = null)
        {
            var ratingsQuery = _dbContext.Ratings
                .AsNoTracking()
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                var parsedScore = int.TryParse(normalizedQuery, out var score);

                ratingsQuery = ratingsQuery.Where(rating =>
                    rating.Player.FirstName.ToLower().Contains(normalizedQuery) ||
                    rating.Player.LastName.ToLower().Contains(normalizedQuery) ||
                    rating.User.FirstName.ToLower().Contains(normalizedQuery) ||
                    rating.User.LastName.ToLower().Contains(normalizedQuery) ||
                    rating.Match.HomeTeam.Name.ToLower().Contains(normalizedQuery) ||
                    rating.Match.AwayTeam.Name.ToLower().Contains(normalizedQuery) ||
                    (rating.Comment != null && rating.Comment.ToLower().Contains(normalizedQuery)) ||
                    (parsedScore && rating.Score == score));
            }

            return ratingsQuery
                .OrderByDescending(rating => rating.Score)
                .AsEnumerable()
                .Select(rating => new RatingDetailsViewModel
                {
                    Id = rating.Id,
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
                    UserId = rating.UserId,
                    PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                    MatchDescription = $"{rating.Match.HomeTeam.Name} vs {rating.Match.AwayTeam.Name} on {rating.Match.Date:yyyy-MM-dd}",
                    UserName = $"{rating.User.FirstName} {rating.User.LastName}",
                    Score = rating.Score,
                    Comment = rating.Comment
                })
                .ToList();
        }

        public RatingDetailsViewModel? GetById(int id)
        {
            return _dbContext.Ratings
                .AsNoTracking()
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
                .Where(rating => rating.Id == id)
                .AsEnumerable()
                .Select(rating => new RatingDetailsViewModel
                {
                    Id = rating.Id,
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
                    UserId = rating.UserId,
                    PlayerName = $"{rating.Player.FirstName} {rating.Player.LastName}",
                    MatchDescription = $"{rating.Match.HomeTeam.Name} vs {rating.Match.AwayTeam.Name} on {rating.Match.Date:yyyy-MM-dd}",
                    UserName = $"{rating.User.FirstName} {rating.User.LastName}",
                    Score = rating.Score,
                    Comment = rating.Comment
                })
                .FirstOrDefault();
        }

        public RatingFormViewModel BuildFormModel()
        {
            var model = new RatingFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public RatingFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Ratings
                .AsNoTracking()
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
                .Where(rating => rating.Id == id)
                .Select(rating => new RatingFormViewModel
                {
                    Id = rating.Id,
                    // Derive the cascade selections from the rating's match.
                    LeagueId = rating.Match.LeagueId,
                    HomeTeamId = rating.Match.HomeTeamId,
                    AwayTeamId = rating.Match.AwayTeamId,
                    MatchId = rating.MatchId,
                    PlayerId = rating.PlayerId,
                    UserId = rating.UserId,
                    Score = rating.Score,
                    Comment = rating.Comment,
                    PlayerLabel = rating.Player.FirstName + " " + rating.Player.LastName,
                    MatchLabel = rating.Match.HomeTeam.Name + " vs " + rating.Match.AwayTeam.Name + " on " + rating.Match.Date.ToString("yyyy-MM-dd"),
                    UserLabel = rating.User.FirstName + " " + rating.User.LastName
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(RatingFormViewModel model)
        {
            // Always available.
            model.LeagueOptions = ToSelectList(GetLeagues(), model.LeagueId);
            model.UserOptions = ToSelectList(GetUsers(), model.UserId);

            // Each dependent list is only built when its parent value is known, so a
            // fresh Create form renders the downstream selects empty (and disabled),
            // while Edit / invalid-postback re-renders them fully and pre-selected.
            if (model.LeagueId.HasValue)
            {
                var clubs = GetClubsInLeague(model.LeagueId.Value);
                model.HomeTeamOptions = ToSelectList(clubs, model.HomeTeamId);

                var awayClubs = model.HomeTeamId.HasValue
                    ? clubs.Where(club => club.Id != model.HomeTeamId.Value).ToList()
                    : clubs;
                model.AwayTeamOptions = ToSelectList(awayClubs, model.AwayTeamId);
            }

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue)
            {
                model.MatchOptions = ToSelectList(GetMatchesBetween(model.HomeTeamId.Value, model.AwayTeamId.Value), model.MatchId);
            }

            if (model.MatchId.HasValue)
            {
                model.PlayerOptions = ToSelectList(GetPlayersForMatch(model.MatchId.Value), model.PlayerId);
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

        public List<CascadeOptionViewModel> GetMatchesBetween(int homeTeamId, int awayTeamId)
        {
            return _dbContext.Matches
                .AsNoTracking()
                .Where(match => match.HomeTeamId == homeTeamId && match.AwayTeamId == awayTeamId)
                .OrderByDescending(match => match.Date)
                .AsEnumerable()
                .Select(match => new CascadeOptionViewModel
                {
                    Id = match.Id,
                    Label = $"{match.Date:yyyy-MM-dd} · {match.HomeGoals}–{match.AwayGoals}"
                })
                .ToList();
        }

        public List<CascadeOptionViewModel> GetPlayersForMatch(int matchId)
        {
            var match = _dbContext.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefault(m => m.Id == matchId);

            if (match == null)
            {
                return [];
            }

            var clubIds = new[] { match.HomeTeamId, match.AwayTeamId };

            var players = _dbContext.Players
                .AsNoTracking()
                .Where(player => clubIds.Contains(player.ClubId))
                .OrderBy(player => player.LastName)
                .ThenBy(player => player.FirstName)
                .Select(player => new { player.Id, player.FirstName, player.LastName, player.ClubId })
                .ToList();

            // Home squad first, then away squad, each under its own group header.
            return players
                .Select(player => new
                {
                    IsHome = player.ClubId == match.HomeTeamId,
                    Option = new CascadeOptionViewModel
                    {
                        Id = player.Id,
                        Label = $"{player.FirstName} {player.LastName}",
                        Group = player.ClubId == match.HomeTeamId
                            ? $"Home · {match.HomeTeam.Name}"
                            : $"Away · {match.AwayTeam.Name}"
                    }
                })
                .OrderByDescending(entry => entry.IsHome)
                .Select(entry => entry.Option)
                .ToList();
        }

        public bool LeagueExists(int leagueId)
        {
            return _dbContext.Leagues.Any(league => league.Id == leagueId);
        }

        public bool ClubInLeague(int clubId, int leagueId)
        {
            return _dbContext.Clubs.Any(club => club.Id == clubId && club.LeagueId == leagueId);
        }

        public bool MatchHasTeams(int matchId, int leagueId, int homeTeamId, int awayTeamId)
        {
            return _dbContext.Matches.Any(match =>
                match.Id == matchId &&
                match.LeagueId == leagueId &&
                match.HomeTeamId == homeTeamId &&
                match.AwayTeamId == awayTeamId);
        }

        public bool PlayerExists(int playerId)
        {
            return _dbContext.Players.Any(player => player.Id == playerId);
        }

        public bool IsPlayerInMatch(int playerId, int matchId)
        {
            var playerClubId = _dbContext.Players
                .AsNoTracking()
                .Where(player => player.Id == playerId)
                .Select(player => (int?)player.ClubId)
                .FirstOrDefault();

            if (!playerClubId.HasValue)
            {
                return false;
            }

            return _dbContext.Matches
                .AsNoTracking()
                .Any(match => match.Id == matchId && (match.HomeTeamId == playerClubId.Value || match.AwayTeamId == playerClubId.Value));
        }

        public bool UserExists(int userId)
        {
            return _dbContext.Users.Any(user => user.Id == userId);
        }

        public int Add(RatingFormViewModel model)
        {
            var entity = new Rating
            {
                PlayerId = model.PlayerId!.Value,
                MatchId = model.MatchId!.Value,
                UserId = model.UserId!.Value,
                Score = model.Score,
                Comment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim()
            };

            _dbContext.Ratings.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, RatingFormViewModel model)
        {
            var entity = _dbContext.Ratings.FirstOrDefault(rating => rating.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.PlayerId = model.PlayerId!.Value;
            entity.MatchId = model.MatchId!.Value;
            entity.UserId = model.UserId!.Value;
            entity.Score = model.Score;
            entity.Comment = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Ratings.FirstOrDefault(rating => rating.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Ratings.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private List<CascadeOptionViewModel> GetLeagues()
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(league => league.Name)
                .Select(league => new CascadeOptionViewModel
                {
                    Id = league.Id,
                    Label = league.Name
                })
                .ToList();
        }

        private List<CascadeOptionViewModel> GetUsers()
        {
            return _dbContext.Users
                .AsNoTracking()
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .Select(user => new CascadeOptionViewModel
                {
                    Id = user.Id,
                    Label = user.FirstName + " " + user.LastName
                })
                .ToList();
        }

        private static List<SelectListItem> ToSelectList(IEnumerable<CascadeOptionViewModel> options, int? selectedId)
        {
            var groups = new Dictionary<string, SelectListGroup>();
            var items = new List<SelectListItem>();

            foreach (var option in options)
            {
                SelectListGroup? group = null;

                if (!string.IsNullOrEmpty(option.Group))
                {
                    if (!groups.TryGetValue(option.Group, out group))
                    {
                        group = new SelectListGroup { Name = option.Group };
                        groups[option.Group] = group;
                    }
                }

                items.Add(new SelectListItem
                {
                    Value = option.Id.ToString(),
                    Text = option.Label,
                    Selected = selectedId == option.Id,
                    Group = group
                });
            }

            return items;
        }
    }
}
