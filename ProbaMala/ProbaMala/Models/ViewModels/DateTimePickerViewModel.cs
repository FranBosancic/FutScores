namespace ProbaMala.Models.ViewModels
{
    public class DateTimePickerViewModel
    {
        public string InputName { get; set; } = null!;
        public string InputId { get; set; } = null!;
        public DateTime? Value { get; set; }
        public string Placeholder { get; set; } = "Select date and time";
        public bool EnableTime { get; set; } = true;
        public int MinuteIncrement { get; set; } = 5;
    }
}