namespace ProbaMala.Models.ViewModels
{
    public class AutocompleteFieldViewModel
    {
        public string Label { get; set; } = null!;
        public string HiddenInputName { get; set; } = null!;
        public string HiddenInputId { get; set; } = null!;
        public int? HiddenValue { get; set; }
        public string VisibleInputName { get; set; } = null!;
        public string VisibleInputId { get; set; } = null!;
        public string? VisibleValue { get; set; }
        public string SearchUrl { get; set; } = null!;
        public string Placeholder { get; set; } = null!;
        public string EmptyStateText { get; set; } = "No matches found.";
    }
}