using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProbaMala.Services;

namespace ProbaMala.Controllers
{
    // Global search. Public — anyone can search the pages/menus. Returns the
    // _SearchResults partial so the header dropdown can drop the HTML straight in.
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET /search?q=...   (Croatian alias: /pretraga)
        [HttpGet("/search", Name = "global-search")]
        [HttpGet("/pretraga")]
        public IActionResult Suggest(string? q)
        {
            return PartialView("_SearchResults", _searchService.Search(q));
        }
    }
}
