namespace ProbaMala.Models.DTOs
{
    // Structured rating-author (domain User) data the AI extracts from a note.
    // No FK resolution needed — these fields map straight onto the form.
    public class UserAiIntent
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
