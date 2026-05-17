using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IClubRepository
    {
        List<ClubDetailsViewModel> GetAll(string? query = null);
        ClubDetailsViewModel? GetById(int id);
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

        public List<ClubDetailsViewModel> GetAll(string? query = null)
        {
            var clubsQuery = _dbContext.Clubs
                .AsNoTracking()
                .Include(club => club.League)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                var parsedYear = int.TryParse(normalizedQuery, out var foundedYear);

                clubsQuery = clubsQuery.Where(club =>
                    club.Name.ToLower().Contains(normalizedQuery) ||
                    club.League.Name.ToLower().Contains(normalizedQuery) ||
                    (parsedYear && club.FoundedDate.Year == foundedYear));
            }

            return clubsQuery
                .OrderBy(club => club.Name)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name,
                    PlayerCount = club.Players.Count,
                    MatchCount = club.HomeMatches.Count + club.AwayMatches.Count,
                    CanDelete = club.Players.Count == 0 && club.HomeMatches.Count == 0 && club.AwayMatches.Count == 0
                })
                .ToList();
        }

        public ClubDetailsViewModel? GetById(int id)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Include(club => club.League)
                .Where(club => club.Id == id)
                .Select(club => new ClubDetailsViewModel
                {
                    Id = club.Id,
                    LeagueId = club.LeagueId,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueName = club.League.Name,
                    PlayerCount = club.Players.Count,
                    MatchCount = club.HomeMatches.Count + club.AwayMatches.Count,
                    CanDelete = club.Players.Count == 0 && club.HomeMatches.Count == 0 && club.AwayMatches.Count == 0
                })
                .FirstOrDefault();
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
                .Where(club => club.Id == id)
                .Select(club => new ClubFormViewModel
                {
                    Id = club.Id,
                    Name = club.Name,
                    FoundedDate = club.FoundedDate,
                    LeagueId = club.LeagueId
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(ClubFormViewModel model)
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
        }

        public bool NameExists(string name, int? excludeClubId = null)
        {
            var normalizedName = name.Trim().ToLower();

            return _dbContext.Clubs.Any(club =>
                club.Name.ToLower() == normalizedName &&
                (!excludeClubId.HasValue || club.Id != excludeClubId.Value));
        }

        public bool LeagueExists(int leagueId)
        {
            return _dbContext.Leagues.Any(league => league.Id == leagueId);
        }

        public bool CanDelete(int id)
        {
            return _dbContext.Clubs
                .AsNoTracking()
                .Where(club => club.Id == id)
                .All(club => club.Players.Count == 0 && club.HomeMatches.Count == 0 && club.AwayMatches.Count == 0);
        }

        public int Add(ClubFormViewModel model)
        {
            var entity = new Club
            {
                Name = model.Name.Trim(),
                FoundedDate = model.FoundedDate,
                LeagueId = model.LeagueId!.Value
            };

            _dbContext.Clubs.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, ClubFormViewModel model)
        {
            var entity = _dbContext.Clubs.FirstOrDefault(club => club.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.Name = model.Name.Trim();
            entity.FoundedDate = model.FoundedDate;
            entity.LeagueId = model.LeagueId!.Value;

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Clubs.FirstOrDefault(club => club.Id == id);

            if (entity == null || !CanDelete(id))
            {
                return false;
            }

            _dbContext.Clubs.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}