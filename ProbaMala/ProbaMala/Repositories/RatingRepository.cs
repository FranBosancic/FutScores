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
        List<RatingDetailsViewModel> GetByUserId(int userId);
        RatingDetailsViewModel? GetById(int id);
        RatingFormViewModel BuildFormModel(int? matchId = null, int? playerId = null);
        RatingFormViewModel? GetFormById(int id);
        void PopulateFormOptions(RatingFormViewModel model);

        // JSON endpoints used by the cascade dropdown (League → Home → Away → Match → Player)
        List<CascadeOptionViewModel> GetClubsInLeague(int leagueId, int? excludeClubId = null);
        List<CascadeOptionViewModel> GetMatchesBetween(int homeTeamId, int awayTeamId);
        List<CascadeOptionViewModel> GetPlayersForMatch(int matchId);

        // Validation helpers — used in the controller to check server-side consistency
        bool LeagueExists(int leagueId);
        bool ClubInLeague(int clubId, int leagueId);
        bool MatchHasTeams(int matchId, int leagueId, int homeTeamId, int awayTeamId);
        bool PlayerExists(int playerId);
        bool IsPlayerInMatch(int playerId, int matchId);
        bool UserExists(int userId);

        // Resolves the rating-author profile (domain User) tied to a login (AppUser).
        // GetProfileIdForAppUser only reads (used for ownership checks); GetOrCreate
        // makes a minimal profile when a login doesn't have one yet (used when rating).
        int? GetProfileIdForAppUser(string appUserId);
        int GetOrCreateProfileId(string appUserId, string email);

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
                .Include(r => r.Player)
                .Include(r => r.User)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                var parsedScore = int.TryParse(q, out var score);

                ratingsQuery = ratingsQuery.Where(r =>
                    r.Player.FirstName.ToLower().Contains(q) ||
                    r.Player.LastName.ToLower().Contains(q) ||
                    r.User.FirstName.ToLower().Contains(q) ||
                    r.User.LastName.ToLower().Contains(q) ||
                    r.Match.HomeTeam.Name.ToLower().Contains(q) ||
                    r.Match.AwayTeam.Name.ToLower().Contains(q) ||
                    (r.Comment != null && r.Comment.ToLower().Contains(q)) ||
                    (parsedScore && r.Score == score));
            }

            // Newest matches first; ties (ratings on the same match) fall back to
            // most-recently-added (highest Id) for a stable order.
            return ratingsQuery
                .OrderByDescending(r => r.Match.Date)
                .ThenByDescending(r => r.Id)
                .AsEnumerable()
                .Select(r => MapToDetailsViewModel(r))
                .ToList();
        }

        public RatingDetailsViewModel? GetById(int id)
        {
            var rating = _dbContext.Ratings
                .AsNoTracking()
                .Include(r => r.Player)
                .Include(r => r.User)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                .FirstOrDefault(r => r.Id == id);

            if (rating == null)
                return null;

            return MapToDetailsViewModel(rating);
        }

        // All ratings authored by one profile (domain User), newest match first.
        // Used by the "your ratings" page.
        public List<RatingDetailsViewModel> GetByUserId(int userId)
        {
            return _dbContext.Ratings
                .AsNoTracking()
                .Include(r => r.Player)
                .Include(r => r.User)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Match.Date)
                .ThenByDescending(r => r.Id)
                .AsEnumerable()
                .Select(MapToDetailsViewModel)
                .ToList();
        }

        // Builds a form model, optionally pre-filled from a known match and player.
        // This is used when the user clicks the "Rate" button on a match squad row —
        // the whole cascade is pre-selected so they only need to set the score.
        public RatingFormViewModel BuildFormModel(int? matchId = null, int? playerId = null)
        {
            var model = new RatingFormViewModel();

            if (matchId.HasValue)
            {
                // Load just the IDs we need; no need to load the full navigation properties here
                var match = _dbContext.Matches
                    .AsNoTracking()
                    .Where(m => m.Id == matchId.Value)
                    .Select(m => new { m.Id, m.LeagueId, m.HomeTeamId, m.AwayTeamId })
                    .FirstOrDefault();

                if (match != null)
                {
                    model.LeagueId   = match.LeagueId;
                    model.HomeTeamId = match.HomeTeamId;
                    model.AwayTeamId = match.AwayTeamId;
                    model.MatchId    = match.Id;

                    // Only honour the player pre-selection if that player
                    // actually plays for one of the two clubs in this match
                    if (playerId.HasValue && IsPlayerInMatch(playerId.Value, match.Id))
                    {
                        model.PlayerId = playerId.Value;
                    }
                }
            }

            PopulateFormOptions(model);
            return model;
        }

        public RatingFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Ratings
                .AsNoTracking()
                .Include(r => r.Player)
                .Include(r => r.User)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                .Where(r => r.Id == id)
                .Select(r => new RatingFormViewModel
                {
                    Id         = r.Id,
                    // The cascade fields (league, home, away) aren't stored on Rating —
                    // they're derived from the match so the form re-renders correctly on Edit
                    LeagueId   = r.Match.LeagueId,
                    HomeTeamId = r.Match.HomeTeamId,
                    AwayTeamId = r.Match.AwayTeamId,
                    MatchId    = r.MatchId,
                    PlayerId   = r.PlayerId,
                    UserId     = r.UserId,
                    Score      = r.Score,
                    Comment    = r.Comment,
                    PlayerLabel = r.Player.FirstName + " " + r.Player.LastName,
                    MatchLabel  = r.Match.HomeTeam.Name + " vs " + r.Match.AwayTeam.Name + " on " + r.Match.Date.ToString("yyyy-MM-dd"),
                    UserLabel   = r.User.FirstName + " " + r.User.LastName
                })
                .FirstOrDefault();

            if (model == null)
                return null;

            PopulateFormOptions(model);
            return model;
        }

        // Fills all dropdown lists on the form.
        // Dependent lists are only built when their parent value is known —
        // this way a fresh Create form starts with most dropdowns empty/disabled,
        // and the cascade JS fills them in as the user makes selections.
        public void PopulateFormOptions(RatingFormViewModel model)
        {
            model.LeagueOptions = ToSelectList(GetLeagues(), model.LeagueId);
            model.UserOptions   = ToSelectList(GetUsers(), model.UserId);

            if (model.LeagueId.HasValue)
            {
                var clubs = GetClubsInLeague(model.LeagueId.Value);
                model.HomeTeamOptions = ToSelectList(clubs, model.HomeTeamId);

                var awayClubs = model.HomeTeamId.HasValue
                    ? clubs.Where(c => c.Id != model.HomeTeamId.Value).ToList()
                    : clubs;
                model.AwayTeamOptions = ToSelectList(awayClubs, model.AwayTeamId);
            }

            if (model.HomeTeamId.HasValue && model.AwayTeamId.HasValue)
            {
                model.MatchOptions = ToSelectList(
                    GetMatchesBetween(model.HomeTeamId.Value, model.AwayTeamId.Value),
                    model.MatchId);
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
                .Where(c => c.LeagueId == leagueId && (excludeClubId == null || c.Id != excludeClubId))
                .OrderBy(c => c.Name)
                .Select(c => new CascadeOptionViewModel { Id = c.Id, Label = c.Name })
                .ToList();
        }

        public List<CascadeOptionViewModel> GetMatchesBetween(int homeTeamId, int awayTeamId)
        {
            return _dbContext.Matches
                .AsNoTracking()
                .Where(m => m.HomeTeamId == homeTeamId && m.AwayTeamId == awayTeamId)
                .OrderByDescending(m => m.Date)
                .AsEnumerable()
                .Select(m => new CascadeOptionViewModel
                {
                    Id    = m.Id,
                    Label = $"{m.Date:yyyy-MM-dd} · {m.HomeGoals}–{m.AwayGoals}"
                })
                .ToList();
        }

        // Returns all players from both clubs in a match, grouped under
        // "Home · ClubName" and "Away · ClubName" option group headers.
        public List<CascadeOptionViewModel> GetPlayersForMatch(int matchId)
        {
            var match = _dbContext.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefault(m => m.Id == matchId);

            if (match == null)
                return [];

            var clubIds = new[] { match.HomeTeamId, match.AwayTeamId };

            var players = _dbContext.Players
                .AsNoTracking()
                .Where(p => clubIds.Contains(p.ClubId))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new { p.Id, p.FirstName, p.LastName, p.ClubId })
                .ToList();

            // Home squad first, then away squad
            return players
                .OrderByDescending(p => p.ClubId == match.HomeTeamId)
                .Select(p => new CascadeOptionViewModel
                {
                    Id    = p.Id,
                    Label = $"{p.FirstName} {p.LastName}",
                    Group = p.ClubId == match.HomeTeamId
                        ? $"Home · {match.HomeTeam.Name}"
                        : $"Away · {match.AwayTeam.Name}"
                })
                .ToList();
        }

        public bool LeagueExists(int leagueId) =>
            _dbContext.Leagues.Any(l => l.Id == leagueId);

        public bool ClubInLeague(int clubId, int leagueId) =>
            _dbContext.Clubs.Any(c => c.Id == clubId && c.LeagueId == leagueId);

        public bool MatchHasTeams(int matchId, int leagueId, int homeTeamId, int awayTeamId) =>
            _dbContext.Matches.Any(m =>
                m.Id == matchId &&
                m.LeagueId == leagueId &&
                m.HomeTeamId == homeTeamId &&
                m.AwayTeamId == awayTeamId);

        public bool PlayerExists(int playerId) =>
            _dbContext.Players.Any(p => p.Id == playerId);

        // Checks whether the player belongs to either club that played in this match.
        // Used to reject "Rate" pre-fills for players who aren't part of the match.
        public bool IsPlayerInMatch(int playerId, int matchId)
        {
            var clubId = _dbContext.Players
                .AsNoTracking()
                .Where(p => p.Id == playerId)
                .Select(p => (int?)p.ClubId)
                .FirstOrDefault();

            if (!clubId.HasValue)
                return false;

            return _dbContext.Matches
                .AsNoTracking()
                .Any(m => m.Id == matchId && (m.HomeTeamId == clubId.Value || m.AwayTeamId == clubId.Value));
        }

        public bool UserExists(int userId) =>
            _dbContext.Users.Any(u => u.Id == userId);

        public int? GetProfileIdForAppUser(string appUserId) =>
            _dbContext.Users.AsNoTracking()
                .Where(u => u.AppUserId == appUserId)
                .Select(u => (int?)u.Id)
                .FirstOrDefault();

        public int GetOrCreateProfileId(string appUserId, string email)
        {
            var existing = _dbContext.Users.FirstOrDefault(u => u.AppUserId == appUserId);
            if (existing != null)
                return existing.Id;

            // A login without a profile yet (the seeded admin or an external-login
            // account): create a minimal one derived from the email's local part.
            var localPart = email.Contains('@') ? email[..email.IndexOf('@')] : email;
            var profile = new User
            {
                FirstName = string.IsNullOrWhiteSpace(localPart) ? "User" : localPart,
                LastName = "(account)",
                Email = email,
                AppUserId = appUserId
            };
            _dbContext.Users.Add(profile);
            _dbContext.SaveChanges();
            return profile.Id;
        }

        public int Add(RatingFormViewModel model)
        {
            var entity = new Rating
            {
                PlayerId = model.PlayerId!.Value,
                MatchId  = model.MatchId!.Value,
                UserId   = model.UserId!.Value,
                Score    = model.Score,
                Comment  = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim()
            };

            _dbContext.Ratings.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, RatingFormViewModel model)
        {
            var entity = _dbContext.Ratings.FirstOrDefault(r => r.Id == id);

            if (entity == null)
                return false;

            entity.PlayerId = model.PlayerId!.Value;
            entity.MatchId  = model.MatchId!.Value;
            entity.UserId   = model.UserId!.Value;
            entity.Score    = model.Score;
            entity.Comment  = string.IsNullOrWhiteSpace(model.Comment) ? null : model.Comment.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Ratings.FirstOrDefault(r => r.Id == id);

            if (entity == null)
                return false;

            _dbContext.Ratings.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        // ── Private helpers ──────────────────────────────────────────────────

        // Maps a fully-loaded Rating entity to the view model used by the list and details pages.
        private static RatingDetailsViewModel MapToDetailsViewModel(Rating r)
        {
            return new RatingDetailsViewModel
            {
                Id               = r.Id,
                PlayerId         = r.PlayerId,
                MatchId          = r.MatchId,
                UserId           = r.UserId,
                PlayerName       = $"{r.Player.FirstName} {r.Player.LastName}",
                MatchDescription = $"{r.Match.HomeTeam.Name} vs {r.Match.AwayTeam.Name} on {r.Match.Date:yyyy-MM-dd}",
                UserName         = $"{r.User.FirstName} {r.User.LastName}",
                Score            = r.Score,
                Comment          = r.Comment,
                HomeTeamName     = r.Match.HomeTeam.Name,
                AwayTeamName     = r.Match.AwayTeam.Name,
                HomeGoals        = r.Match.HomeGoals,
                AwayGoals        = r.Match.AwayGoals,
                MatchDate        = r.Match.Date
            };
        }

        private List<CascadeOptionViewModel> GetLeagues()
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new CascadeOptionViewModel { Id = l.Id, Label = l.Name })
                .ToList();
        }

        private List<CascadeOptionViewModel> GetUsers()
        {
            return _dbContext.Users
                .AsNoTracking()
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new CascadeOptionViewModel { Id = u.Id, Label = u.FirstName + " " + u.LastName })
                .ToList();
        }

        // Converts CascadeOptionViewModels to SelectListItems, supporting optgroup headers.
        private static List<SelectListItem> ToSelectList(IEnumerable<CascadeOptionViewModel> options, int? selectedId)
        {
            var groups = new Dictionary<string, SelectListGroup>();
            var items  = new List<SelectListItem>();

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
                    Value    = option.Id.ToString(),
                    Text     = option.Label,
                    Selected = selectedId == option.Id,
                    Group    = group
                });
            }

            return items;
        }
    }
}
