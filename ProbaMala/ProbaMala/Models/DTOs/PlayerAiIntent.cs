namespace ProbaMala.Models.DTOs
{
    // Structured player profile the AI extracts from a natural-language note.
    // ClubName is a name (resolved to a ClubId by our own code); Position/DateOfBirth
    // are parsed in the controller.
    public class PlayerAiIntent
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? DateOfBirth { get; set; }
        public string? Position { get; set; }
        public string Nationality { get; set; } = "";
        public string ClubName { get; set; } = "";
    }
}
