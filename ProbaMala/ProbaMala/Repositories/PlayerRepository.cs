using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IPlayerRepository
    {
        List<PlayerDetailsViewModel> GetAll(string? query = null);
        PlayerDetailsViewModel? GetById(int id);
        PlayerFormViewModel BuildFormModel();
        PlayerFormViewModel? GetFormById(int id);
        void PopulateFormOptions(PlayerFormViewModel model);
        bool ClubExists(int clubId);
        int Add(PlayerFormViewModel model);
        bool Update(int id, PlayerFormViewModel model);
        bool Delete(int id);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _dbContext;

        public PlayerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<PlayerDetailsViewModel> GetAll(string? query = null)
        {
            var playersQuery = _dbContext.Players
                .AsNoTracking()
                .Include(p => p.Club)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();

                // Match position by name in memory (only 4 values): EF/Npgsql can't
                // translate enum.ToString() to SQL, so we turn it into an IN filter.
                var matchedPositions = Enum.GetValues<Position>()
                    .Where(position => position.ToString().ToLower().Contains(q))
                    .ToList();

                playersQuery = playersQuery.Where(p =>
                    p.FirstName.ToLower().Contains(q) ||
                    p.LastName.ToLower().Contains(q) ||
                    p.Nationality.ToLower().Contains(q) ||
                    p.Club.Name.ToLower().Contains(q) ||
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
                    ClubName    = p.Club.Name,
                    // Count must happen inside the EF query (before materialisation),
                    // otherwise it would always return 0 when lazy loading is disabled
                    RatingCount = p.Ratings.Count,
                    PhotoUrl    = p.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault()
                })
                .ToList();
        }

        // Returns the full player profile including their rating history.
        // We use .AsEnumerable() here because the nested projection (building
        // RatingDetailsViewModel) can't be fully translated to SQL.
        public PlayerDetailsViewModel? GetById(int id)
        {
            var player = _dbContext.Players
                .AsNoTracking()
                .Include(p => p.Club)
                .Include(p => p.Images)
                .Include(p => p.Ratings).ThenInclude(r => r.User)
                .Include(p => p.Ratings).ThenInclude(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(p => p.Ratings).ThenInclude(r => r.Match).ThenInclude(m => m.AwayTeam)
                .FirstOrDefault(p => p.Id == id);

            if (player == null)
                return null;

            return new PlayerDetailsViewModel
            {
                Id          = player.Id,
                ClubId      = player.ClubId,
                FirstName   = player.FirstName,
                LastName    = player.LastName,
                DateOfBirth = player.DateOfBirth,
                Position    = player.Position,
                Nationality = player.Nationality,
                ClubName    = player.Club.Name,
                RatingCount = player.Ratings.Count,
                PhotoUrl    = player.Images.Where(i => i.IsPrimary).Select(i => i.FilePath).FirstOrDefault(),
                Ratings     = player.Ratings
                    .OrderByDescending(r => r.Match.Date)
                    .Select(r => new RatingDetailsViewModel
                    {
                        Id               = r.Id,
                        PlayerId         = r.PlayerId,
                        MatchId          = r.MatchId,
                        UserId           = r.UserId,
                        PlayerName       = $"{player.FirstName} {player.LastName}",
                        MatchDescription = $"{r.Match.HomeTeam.Name} vs {r.Match.AwayTeam.Name} on {r.Match.Date:yyyy-MM-dd}",
                        UserName         = $"{r.User.FirstName} {r.User.LastName}",
                        Score            = r.Score,
                        Comment          = r.Comment,
                        HomeTeamName     = r.Match.HomeTeam.Name,
                        AwayTeamName     = r.Match.AwayTeam.Name,
                        HomeGoals        = r.Match.HomeGoals,
                        AwayGoals        = r.Match.AwayGoals,
                        MatchDate        = r.Match.Date
                    })
                    .ToList()
            };
        }

        public PlayerFormViewModel BuildFormModel()
        {
            var model = new PlayerFormViewModel();
            PopulateFormOptions(model);
            return model;
        }

        public PlayerFormViewModel? GetFormById(int id)
        {
            var model = _dbContext.Players
                .AsNoTracking()
                .Where(player => player.Id == id)
                .Select(player => new PlayerFormViewModel
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    Position = player.Position,
                    ClubId = player.ClubId,
                    Nationality = player.Nationality
                })
                .FirstOrDefault();

            if (model == null)
            {
                return null;
            }

            PopulateFormOptions(model);
            return model;
        }

        public void PopulateFormOptions(PlayerFormViewModel model)
        {
            model.ClubOptions = _dbContext.Clubs
                .AsNoTracking()
                .OrderBy(club => club.Name)
                .Select(club => new SelectListItem
                {
                    Value = club.Id.ToString(),
                    Text = club.Name,
                    Selected = model.ClubId == club.Id
                })
                .ToList();

            model.PositionOptions = Enum.GetValues<Position>()
                .Select(position => new SelectListItem
                {
                    Value = position.ToString(),
                    Text = position.ToString(),
                    Selected = model.Position == position
                })
                .ToList();
        }

        public bool ClubExists(int clubId)
        {
            return _dbContext.Clubs.Any(club => club.Id == clubId);
        }

        public int Add(PlayerFormViewModel model)
        {
            var entity = new Player
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                DateOfBirth = model.DateOfBirth,
                Position = model.Position!.Value,
                ClubId = model.ClubId!.Value,
                Nationality = model.Nationality.Trim()
            };

            _dbContext.Players.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public bool Update(int id, PlayerFormViewModel model)
        {
            var entity = _dbContext.Players.FirstOrDefault(player => player.Id == id);

            if (entity == null)
            {
                return false;
            }

            entity.FirstName = model.FirstName.Trim();
            entity.LastName = model.LastName.Trim();
            entity.DateOfBirth = model.DateOfBirth;
            entity.Position = model.Position!.Value;
            entity.ClubId = model.ClubId!.Value;
            entity.Nationality = model.Nationality.Trim();

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.Players.FirstOrDefault(player => player.Id == id);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Players.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }
    }
}