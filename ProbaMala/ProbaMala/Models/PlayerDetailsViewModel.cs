namespace ProbaMala.Models
{
    public class PlayerDetailsViewModel
    {
        public int Id { get; set; }
        public int ClubId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public Position Position { get; set; }
        public string Nationality { get; set; } = null!;
        public string ClubName { get; set; } = null!;
    }
}