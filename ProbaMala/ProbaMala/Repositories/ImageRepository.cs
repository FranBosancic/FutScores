using Microsoft.EntityFrameworkCore;
using ProbaMala.Data;
using ProbaMala.Models.Entities;
using ProbaMala.Models.ViewModels;

namespace ProbaMala.Repositories
{
    public interface IImageRepository
    {
        bool OwnerExists(ImageOwnerType owner, int ownerId);
        List<ImageListItemViewModel> GetForOwner(ImageOwnerType owner, int ownerId);

        // Saves the uploaded file to disk and stores its metadata. Returns the created
        // image, or an error message when the owner is missing or the file is invalid.
        Task<(Image? image, string? error)> AddAsync(ImageOwnerType owner, int ownerId, IFormFile? file);

        bool Delete(int imageId);
        bool SetPrimary(int imageId);
    }

    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        // Whitelist of what we accept. We check both the extension and the reported
        // content type so a client can't slip a non-image through on either alone.
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private static readonly string[] AllowedContentTypes =
            { "image/jpeg", "image/png", "image/webp", "image/gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public ImageRepository(AppDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        public bool OwnerExists(ImageOwnerType owner, int ownerId) => owner switch
        {
            ImageOwnerType.Club   => _dbContext.Clubs.Any(c => c.Id == ownerId),
            ImageOwnerType.Player => _dbContext.Players.Any(p => p.Id == ownerId),
            _                     => false
        };

        public List<ImageListItemViewModel> GetForOwner(ImageOwnerType owner, int ownerId)
        {
            var query = owner == ImageOwnerType.Club
                ? _dbContext.Images.Where(i => i.ClubId == ownerId)
                : _dbContext.Images.Where(i => i.PlayerId == ownerId);

            return query
                .AsNoTracking()
                .OrderByDescending(i => i.IsPrimary)
                .ThenByDescending(i => i.CreatedAt)
                .Select(i => new ImageListItemViewModel
                {
                    Id        = i.Id,
                    FileName  = i.FileName,
                    Url       = i.FilePath,
                    FileSize  = i.FileSize,
                    IsPrimary = i.IsPrimary
                })
                .ToList();
        }

        public async Task<(Image? image, string? error)> AddAsync(ImageOwnerType owner, int ownerId, IFormFile? file)
        {
            if (!OwnerExists(owner, ownerId))
                return (null, $"{owner} not found.");

            // One image per owner: reject the upload if there's already one. The user
            // must delete the existing image before uploading a replacement.
            var alreadyHasImage = owner == ImageOwnerType.Club
                ? _dbContext.Images.Any(i => i.ClubId == ownerId)
                : _dbContext.Images.Any(i => i.PlayerId == ownerId);
            if (alreadyHasImage)
                return (null, $"This {owner.ToString().ToLowerInvariant()} already has an image. Delete it before uploading a new one.");

            var validationError = Validate(file);
            if (validationError != null)
                return (null, validationError);

            var folder = owner == ImageOwnerType.Club ? "clubs" : "players";

            // wwwroot/uploads/{clubs|players}/{ownerId}/
            var webRoot     = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDir  = Path.Combine(webRoot, "uploads", folder, ownerId.ToString());
            Directory.CreateDirectory(uploadsDir);

            // Random physical name keeps uploads with identical names from colliding
            // and stops the original name from being used as a path.
            var extension      = Path.GetExtension(file!.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var physicalPath   = Path.Combine(uploadsDir, storedFileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // The first image uploaded for an owner becomes its primary by default.
            var isFirst = !(owner == ImageOwnerType.Club
                ? _dbContext.Images.Any(i => i.ClubId == ownerId)
                : _dbContext.Images.Any(i => i.PlayerId == ownerId));

            var image = new Image
            {
                FileName    = Path.GetFileName(file.FileName),
                FilePath    = $"/uploads/{folder}/{ownerId}/{storedFileName}",
                ContentType = file.ContentType,
                FileSize    = file.Length,
                IsPrimary   = isFirst,
                // Local kind: the column is "timestamp without time zone", which
                // Npgsql refuses to populate from a UTC-kind DateTime.
                CreatedAt   = DateTime.Now,
                ClubId      = owner == ImageOwnerType.Club ? ownerId : null,
                PlayerId    = owner == ImageOwnerType.Player ? ownerId : null
            };

            _dbContext.Images.Add(image);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Don't leave an orphaned file on disk if the metadata save fails.
                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
                throw;
            }

            return (image, null);
        }

        public bool Delete(int imageId)
        {
            var image = _dbContext.Images.FirstOrDefault(i => i.Id == imageId);

            if (image == null)
                return false;

            DeletePhysicalFile(image.FilePath);

            var wasPrimary = image.IsPrimary;
            var clubId     = image.ClubId;
            var playerId   = image.PlayerId;

            _dbContext.Images.Remove(image);
            _dbContext.SaveChanges();

            // If we removed the primary image, promote the most recent remaining one
            // so the owner still has a banner/headshot.
            if (wasPrimary)
            {
                var replacement = _dbContext.Images
                    .Where(i => clubId.HasValue ? i.ClubId == clubId : i.PlayerId == playerId)
                    .OrderByDescending(i => i.CreatedAt)
                    .FirstOrDefault();

                if (replacement != null)
                {
                    replacement.IsPrimary = true;
                    _dbContext.SaveChanges();
                }
            }

            return true;
        }

        public bool SetPrimary(int imageId)
        {
            var image = _dbContext.Images.FirstOrDefault(i => i.Id == imageId);

            if (image == null)
                return false;

            // Clear the flag on the owner's other images, then set it on this one.
            var siblings = _dbContext.Images.Where(i =>
                image.ClubId.HasValue ? i.ClubId == image.ClubId : i.PlayerId == image.PlayerId);

            foreach (var sibling in siblings)
                sibling.IsPrimary = false;

            image.IsPrimary = true;
            _dbContext.SaveChanges();

            return true;
        }

        // Returns an error message when the file is unacceptable, otherwise null.
        private static string? Validate(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return "No file was uploaded.";

            if (file.Length > MaxFileSizeBytes)
                return "File is too large (max 5 MB).";

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return "Unsupported file type. Allowed: jpg, jpeg, png, webp, gif.";

            if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return "Unsupported content type.";

            return null;
        }

        private void DeletePhysicalFile(string webPath)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var physicalPath = Path.Combine(webRoot, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(physicalPath))
                File.Delete(physicalPath);
        }
    }
}
