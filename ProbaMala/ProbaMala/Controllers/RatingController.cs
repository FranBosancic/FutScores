using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("ocjene")]
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/ratings", Name = "ratings-index")]
        [HttpGet("~/ratings/list")]
        public IActionResult Index()
        {
            return View(_ratingRepository.GetAll());
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/ratings/{id:int}", Name = "rating-details")]
        [HttpGet("~/ratings/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _ratingRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
