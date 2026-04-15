using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    public class RatingController : Controller
    {
        private readonly RatingMockRepository _ratingRepository;

        public RatingController(RatingMockRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public IActionResult Index()
        {
            var ratings = _ratingRepository.GetAll();
            return View(ratings);
        }

        public IActionResult Details(int id)
        {
            var rating = _ratingRepository.GetById(id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }
    }
}
