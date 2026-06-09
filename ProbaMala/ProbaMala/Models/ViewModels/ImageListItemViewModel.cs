namespace ProbaMala.Models.ViewModels
{
    // Shape rendered by the _ImageList partial — just what the gallery row needs,
    // so the Image entity (and its navigation properties) never reaches the view.
    public class ImageListItemViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public long FileSize { get; set; }
        public bool IsPrimary { get; set; }
    }
}
