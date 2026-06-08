using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface ILeagueRepository
    {
        List<LeagueDetailsViewModel> GetAll(string? query = null);
        LeagueDetailsViewModel? GetById(int id);
        LeagueFormViewModel BuildFormModel();
        LeagueFormViewModel? GetFormById(int id);
        bool NameExists(string name, int? excludeLeagueId = null);
        int Add(LeagueFormViewModel model);
        bool Update(int id, LeagueFormViewModel model);
        bool Delete(int id);
    }

    public class LeagueRepository : ILeagueRepository
    {
        private readonly AppDbContext _dbContext;

        public LeagueRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<LeagueDetailsViewModel> GetAll(string? query = null)
        {
            var leaguesQuery = _dbContext.Leagues.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                leaguesQuery = leaguesQuery.Where(l => l.Name.ToLower().Contains(q));
            }

            return leaguesQuery
                .OrderBy(l => l.Name)
                .Select(l => new LeagueDetailsViewModel
                {
                    Id         = l.Id,
                    Name       = l.Name,
                    // Count inside the EF query so it translates to SQL COUNT(*)
                    ClubCount  = l.Clubs.Count,
                    MatchCount = l.Matches.Count
                })
                .ToList();
        }

        // Loads the league with all its clubs and matches in one round-trip.
        // We build the view model in-memory after the query because the status logic
        // (Finished vs Scheduled) can't be expressed in a SQL projection.
        public LeagueDetailsViewModel? GetById(int id)
        {
            var league = _dbContext.Leagues
                .AsNoTracking()
                .Include(l => l.Clubs).ThenInclude(c => c.Players)
                .Include(l => l.Clubs).ThenInclude(c => c.HomeMatches)
                .Include(l => l.Clubs).ThenInclude(c => c.AwayMatches)
                .Include(l => l.Matches).ThenInclude(m => m.HomeTeam)
                .Include(l => l.Matches).ThenInclude(m => m.AwayTeam)
                .Include(l => l.Matches).ThenInclude(m => m.Ratings)
                .FirstOrDefault(l => l.Id == id);

            if (league == null)
                return null;

            return new LeagueDetailsViewModel
            {
                Id         = league.Id,
                Name       = league.Name,
                ClubCount  = league.Clubs.Count,
                MatchCount = league.Matches.Count,

                Clubs = league.Clubs
                    .OrderBy(c => c.Name)
                    .Select(c => new ClubDetailsViewModel
                    {
                        Id          = c.Id,
                        Name        = c.Name,
                        LeagueId    = league.Id,
                        LeagueName  = league.Name,
                        FoundedDate = c.FoundedDate,
                        PlayerCount = c.Players.Count,
                        MatchCount  = c.HomeMatches.Count + c.AwayMatches.Count
                    })
                    .ToList(),

                // Matches newest-first; past = Finished, future = Scheduled
                Matches = league.Matches
                    .OrderByDescending(m => m.Date)
                    .Select(m => new MatchDetailsViewModel
                    {
                        Id           = m.Id,
                        LeagueId     = league.Id,
                        HomeTeamId   = m.HomeTeamId,
                        AwayTeamId   = m.AwayTeamId,
                        Date         = m.Date,
                        KickoffLabel = m.Date.ToString("dd MMM yyyy"),
                        StatusLabel  = m.Date <= DateTime.UtcNow ? "Finished" : "Scheduled",
                        StatusTone   = m.Date <= DateTime.UtcNow ? "final" : "upcoming",
                        LeagueName   = league.Name,
                        HomeTeamName = m.HomeTeam.Name,
                        AwayTeamName = m.AwayTeam.Name,
                        HomeGoals    = m.HomeGoals,
                        AwayGoals    = m.AwayGoals,
                        RatingCount  = m.Ratings.Count
                    })
                    .ToList()
            };
        }

        public LeagueFormViewModel BuildFormModel()
        {
            return new LeagueFormViewModel();
        }

        public LeagueFormViewModel? GetFormById(int id)
        {
            return _dbContext.Leagues
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new LeagueFormViewModel
                {
                    Id   = l.Id,
                    Name = l.Name
                })
                .FirstOrDefault();
        }

        // Case-insensitive uniqueness check; excludeLeagueId prevents flagging the
        // record's own name during an edit.
        public bool NameExists(string name, int? excludeLeagueId = null)
        {
            var normalized = name.Trim().ToLower();

            return _dbContext.Leagues.Any(l =>
                l.Name.ToLower() == normalized &&
                (!excludeLeagueId.HasValue || l.Id != excludeLeagueId.Value));
        }

        public int Add(LeagueFormViewModel model)
        {
            var entity = new League { Name = model.Name.Trim() };

            _dbContext.Leagues.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, LeagueFormViewModel model)
        {
            var entity = _dbContext.Leagues.FirstOrDefault(l => l.Id == id);

            if (entity == null)
                return false;

            entity.Name = model.Name.Trim();
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Leagues.FirstOrDefault(l => l.Id == id);

            if (entity == null)
                return false;

            _dbContext.Leagues.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
