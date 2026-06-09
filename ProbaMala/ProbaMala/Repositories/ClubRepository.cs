using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IClubRepository
    {
        List<ClubDetailsViewModel> GetAll(string? query = null, int? leagueId = null);
        ClubDetailsViewModel? GetById(int id);
        List<PlayerDetailsViewModel> GetSquad(int clubId, string? q = null);
        ClubFormViewModel BuildFormModel();
        ClubFormViewModel? GetFormById(int id);
        void PopulateFormOptions(ClubFormViewModel model);
        bool NameExists(string name, int? excludeClubId = null);
        bool LeagueExists(int leagueId);
        bool CanDelete(int id);
        int Add(ClubFormViewModel model);
        bool Update(int id, ClubFormViewModel model);
        bool Delete(int id);
    }

    public class ClubRepository : IClubRepository
    {
        private readonly AppDbContext _dbContext;

        public ClubRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ClubDetailsViewModel> GetAll(string? query = null, int? leagueId = null)
        {
            var clubsQuery = _dbContext.Clubs
                .AsNoTracking()
                .Include(c => c.League)
                .AsQueryable();

            // Filter by league when coming from the league nav dropdown
            if (leagueId.HasValue)
                clubsQuery = clubsQuery.Where(c => c.LeagueId == leagueId.Value);

            // Full-text search across name, league, and founded year
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                var parsedYear = int.TryParse(q, out var foundedYear);

                clubsQuery = clubsQuery.Where(c =>
                    c.Name.ToLower().Contains(q) ||
                    c.League.Name.ToLower().Contains(q) ||
                    (parsedYear && c.FoundedDate.Year == foundedYear));
            }

            return clubsQuery
                .OrderBy(c => c.Name)
                .Select(c => new ClubDetailsViewModel
                {
                    Id          = c.Id,
                    LeagueId    = c.LeagueId,
                    Name        = c.Name,
                    FoundedDate = c.FoundedDate,
                    LeagueName  = c.League.Name,
                    // Counts inside the EF query so they translate to SQL COUNT(*)
                    PlayerCount = c.Players.Count,
                    MatchCount  = c.HomeMatches.Count + c.AwayMatches.Count,
                    CanDelete   = c.Players.Count == 0 && c.HomeMatches.Count == 0 && c.AwayMatches.Count == 0,
                    BannerUrl   = c.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault()
                })
                .ToList();
        }

        public ClubDetailsViewModel? GetById(int id)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Include(c => c.League)
                .Where(c => c.Id == id)
                .Select(c => new ClubDetailsViewModel
                {
                    Id          = c.Id,
                    LeagueId    = c.LeagueId,
                    Name        = c.Name,
                    FoundedDate = c.FoundedDate,
                    LeagueName  = c.League.Name,
                    PlayerCount = c.Players.Count,
                    MatchCount  = c.HomeMatches.Count + c.AwayMatches.Count,
                    CanDelete   = c.Players.Count == 0 && c.HomeMatches.Count == 0 && c.AwayMatches.Count == 0,
                    BannerUrl   = c.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault(),
                    Players = c.Players
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName)
                        .Select(p => new PlayerDetailsViewModel
                        {
                            Id          = p.Id,
                            ClubId      = p.ClubId,
                            FirstName   = p.FirstName,
                            LastName    = p.LastName,
                            DateOfBirth = p.DateOfBirth,
                            Position    = p.Position,
                            Nationality = p.Nationality,
                            ClubName    = c.Name,
                            RatingCount = p.Ratings.Count,
                            PhotoUrl    = p.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault()
                        })
                        .ToList()
                })
                .FirstOrDefault();
        }

        public List<PlayerDetailsViewModel> GetSquad(int clubId, string? q = null)
        {
            var playersQuery = _dbContext.Players
                .AsNoTracking()
                .Where(p => p.ClubId == clubId);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qTrim = q.Trim().ToLower();

                // Match position by name in memory (only 4 values): EF/Npgsql can't
                // translate enum.ToString() to SQL, so we turn it into an IN filter.
                var matchedPositions = Enum.GetValues<Position>()
                    .Where(position => position.ToString().ToLower().Contains(qTrim))
                    .ToList();

                playersQuery = playersQuery.Where(p =>
                    p.FirstName.ToLower().Contains(qTrim) ||
                    p.LastName.ToLower().Contains(qTrim) ||
                    p.Nationality.ToLower().Contains(qTrim) ||
                    matchedPositions.Contains(p.Position));
            }

            return playersQuery
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new PlayerDetailsViewModel
                {
                    Id          = p.Id,
                    ClubId      = p.ClubId,
                    FirstName   = p.FirstName,
                    LastName    = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Position    = p.Position,
                    Nationality = p.Nationality,
                    RatingCount = p.Ratings.Count,
                    PhotoUrl    = p.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault()
                })
                .ToList();
        }

        public ClubFormViewModel BuildFormModel()
        {
            var model = new ClubFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public ClubFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Clubs
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ClubFormViewModel
                {
                    Id          = c.Id,
                    Name        = c.Name,
                    FoundedDate = c.FoundedDate,
                    LeagueId    = c.LeagueId
                })
                .FirstOrDefault();

            if (model == null)
                return null;

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(ClubFormViewModel model)
        {
            model.LeagueOptions = _dbContext.Leagues
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .Select(l => new SelectListItem
                {
                    Value    = l.Id.ToString(),
                    Text     = l.Name,
                    Selected = model.LeagueId == l.Id
                })
                .ToList();
        }

        // Case-insensitive uniqueness check; excludeClubId prevents flagging the
        // record's own name during an edit.
        public bool NameExists(string name, int? excludeClubId = null)
        {
            var normalized = name.Trim().ToLower();

            return _dbContext.Clubs.Any(c =>
                c.Name.ToLower() == normalized &&
                (!excludeClubId.HasValue || c.Id != excludeClubId.Value));
        }

        public bool LeagueExists(int leagueId) =>
            _dbContext.Leagues.Any(l => l.Id == leagueId);

        // A club is only deletable when it has no players and no matches.
        public bool CanDelete(int id) =>
            _dbContext.Clubs
                .AsNoTracking()
                .Where(c => c.Id == id)
                .All(c => c.Players.Count == 0 && c.HomeMatches.Count == 0 && c.AwayMatches.Count == 0);

        public int Add(ClubFormViewModel model)
        {
            var entity = new Club
            {
                Name        = model.Name.Trim(),
                FoundedDate = model.FoundedDate,
                LeagueId    = model.LeagueId!.Value
            };

            _dbContext.Clubs.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, ClubFormViewModel model)
        {
            var entity = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);

            if (entity == null)
                return false;

            entity.Name        = model.Name.Trim();
            entity.FoundedDate = model.FoundedDate;
            entity.LeagueId    = model.LeagueId!.Value;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);

            if (entity == null || !CanDelete(id))
                return false;

            _dbContext.Clubs.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
