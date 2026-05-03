using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("utakmice")]
    public class MatchController : Controller
    {
        private readonly IMatchRepository _matchRepository;

        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/matches", Name = "matches-index")]
        [HttpGet("~/matches/list")]
        public IActionResult Index()
        {
            return View(_matchRepository.GetAll());
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/matches/{id:int}", Name = "match-details")]
        [HttpGet("~/matches/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _matchRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
