using Microsoft.AspNetCore.Mvc;
using ProbaMala.Repositories;

namespace ProbaMala.ViewComponents
{
    /// <summary>
    /// Renders the league navigation items shown in the site header.
    /// Each league appears as a hoverable link with a dropdown containing
    /// quick-links to that league's Clubs and Matches pages.
    ///
    /// Using a ViewComponent keeps the layout clean: the _Layout.cshtml
    /// doesn't need to be a controller-backed view just to fetch league data.
    /// </summary>
    public class LeagueNavViewComponent : ViewComponent
    {
        private readonly ILeagueRepository _leagueRepository;

        public LeagueNavViewComponent(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public IViewComponentResult Invoke()
        {
            // Fetch all leagues ordered by name (no search filter — we want everything in the nav)
            var leagues = _leagueRepository.GetAll();
            return View(leagues);
        }
    }
}
