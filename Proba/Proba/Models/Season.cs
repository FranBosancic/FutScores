namespace Proba.Models
{
    public class Season
    {
        public int SeasonId { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public int TotalMatches { get; set; }
        public List<League> Leagues { get; set; }

        public Season()
        {
            Leagues = new List<League>();
        }
    }
}
