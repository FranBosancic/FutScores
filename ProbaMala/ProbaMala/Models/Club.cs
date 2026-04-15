namespace ProbaMala.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime FoundedDate { get; set; }
        public int LeagueId { get; set; }
    }
}