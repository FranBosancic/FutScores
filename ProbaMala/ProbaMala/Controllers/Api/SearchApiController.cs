using Microsoft.AspNetCore.Mvc;
using ProbaMala.Services;

namespace ProbaMala.Controllers.Api
{
    // JSON cross-entity search (pages + data), reusing the same ISearchService the web
    // dropdown uses. The web endpoint (/search) returns an HTML partial; this one returns
    // JSON so API/MCP clients get structured results.
    [Route("api/search")]
    [ApiController]
    public class SearchApiController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchApiController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET /api/search?q=...
        [HttpGet]
        public IActionResult Get([FromQuery] string? q = null) => Ok(_searchService.Search(q));
    }
}
