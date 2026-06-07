namespace ProbaMala.Models.ViewModels
{
    /// <summary>
    /// Lightweight option returned by the rating cascade JSON endpoints
    /// (clubs in a league, matches between two clubs, players in a match).
    /// <see cref="Group"/> is optional and drives client-side &lt;optgroup&gt; headers.
    /// </summary>
    public class CascadeOptionViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = null!;
        public string? Group { get; set; }
    }
}
