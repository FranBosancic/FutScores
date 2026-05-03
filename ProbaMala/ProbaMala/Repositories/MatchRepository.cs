using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IMatchRepository
    {
        List<MatchDetailsViewModel> GetAll();
        MatchDetailsViewModel? GetById(int id);
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly AppDbContext _dbContext;

        public MatchRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MatchDetailsViewModel> GetAll()
        {
            return _dbContext.Matches
                .AsNoTracking()
                .Include(match => match.League)
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .OrderByDescending(match => match.Date)
                .AsEnumerable()
                .Select((match, index) =>
                {
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
                        AwayGoals = match.AwayGoals
                    };
                })
                .ToList();
        }

        public MatchDetailsViewModel? GetById(int id)
        {
            return _dbContext.Matches
                .AsNoTracking()
                .Include(match => match.League)
                .Include(match => match.HomeTeam)
                .Include(match => match.AwayTeam)
                .Where(match => match.Id == id)
                .AsEnumerable()
                .Select(match => new MatchDetailsViewModel
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
                    AwayGoals = match.AwayGoals
                })
                .FirstOrDefault();
        }
    }
}