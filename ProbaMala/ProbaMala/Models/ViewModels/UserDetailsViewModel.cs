namespace ProbaMala.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RatingCount { get; set; }

        // Only populated on the Details page (null on the Index list).
        public List<RatingDetailsViewModel>? Ratings { get; set; }
    }
}