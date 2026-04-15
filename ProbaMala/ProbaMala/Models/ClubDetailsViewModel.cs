namespace ProbaMala.Models
{
    public class ClubDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime FoundedDate { get; set; }
        public string LeagueName { get; set; } = null!;
    }
}