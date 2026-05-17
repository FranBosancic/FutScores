using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IRatingRepository
    {
        List<RatingDetailsViewModel> GetAll();
        RatingDetailsViewModel? GetById(int id);
        RatingFormViewModel BuildFormModel();
        RatingFormViewModel? GetFormById(int id);
        void PopulateFormOptions(RatingFormViewModel model);
        bool PlayerExists(int playerId);
        bool MatchExists(int matchId);
        bool UserExists(int userId);
        bool IsPlayerInMatch(int playerId, int matchId);
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

        public List<RatingDetailsViewModel> GetAll()
        {
            return _dbContext.Ratings
                .AsNoTracking()
                .Include(rating => rating.Player)
                .Include(rating => rating.User)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.HomeTeam)
                .Include(rating => rating.Match)
                    .ThenInclude(match => match.AwayTeam)
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
                    PlayerId = rating.PlayerId,
                    MatchId = rating.MatchId,
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
            model.PlayerOptions = _dbContext.Players
                .AsNoTracking()
                .OrderBy(player => player.LastName)
                .ThenBy(player => player.FirstName)
                .Select(player => new SelectListItem
                {
                    Value = player.Id.ToString(),
                    Text = player.FirstName + " " + player.LastName,
                    Selected = model.PlayerId == player.Id
                })
                .ToList();

            model.MatchOptions = _dbContext.Matches
                .AsNoTracking()
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .OrderByDescending(match => match.Date)
                .AsEnumerable()
                .Select(match => new SelectListItem
                {
                    Value = match.Id.ToString(),
                    Text = $"{match.HomeTeam.Name} vs {match.AwayTeam.Name} on {match.Date:yyyy-MM-dd}",
                    Selected = model.MatchId == match.Id
                })
                .ToList();

            model.UserOptions = _dbContext.Users
                .AsNoTracking()
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .Select(user => new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.FirstName + " " + user.LastName,
                    Selected = model.UserId == user.Id
                })
                .ToList();
        }

        public bool PlayerExists(int playerId)
        {
            return _dbContext.Players.Any(player => player.Id == playerId);
        }

        public bool MatchExists(int matchId)
        {
            return _dbContext.Matches.Any(match => match.Id == matchId);
        }

        public bool UserExists(int userId)
        {
            return _dbContext.Users.Any(user => user.Id == userId);
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
    }
}