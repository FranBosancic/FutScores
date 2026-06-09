namespace ProbaMala.Models.DTOs
{
    // Lightweight DTOs used for nesting inside larger response DTOs.
    // They carry only an identifier and the display fields a client needs to
    // render or link to the related resource — never their own nested graphs,
    // which keeps responses shallow and avoids cyclic/oversized JSON.

    public class LeagueSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ClubSummaryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class PlayerSummaryDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Position { get; set; } = null!;
    }

    public class UserSummaryDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class MatchSummaryDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
    }
}
