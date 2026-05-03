using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IRatingRepository
    {
        List<RatingDetailsViewModel> GetAll();
        RatingDetailsViewModel? GetById(int id);
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
    }
}