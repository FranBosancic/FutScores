using Microsoft.AspNetCore.Mvc;
using ProbaMala.Models.ViewModels;
using ProbaMala.Repositories;

namespace ProbaMala.Controllers
{
    [Route("klubovi")]
    public class ClubController : Controller
    {
        private readonly IClubRepository _clubRepository;

        public ClubController(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;
        }

        [HttpGet("")]
        [HttpGet("popis")]
        [HttpGet("~/clubs", Name = "clubs-index")]
        [HttpGet("~/clubs/list")]
        public IActionResult Index()
        {
            return View(_clubRepository.GetAll());
        }

        [HttpGet("{id:int}")]
        [HttpGet("detalji/{id:int}")]
        [HttpGet("~/clubs/{id:int}", Name = "club-details")]
        [HttpGet("~/clubs/details/{id:int}")]
        public IActionResult Details(int id)
        {
            var viewModel = _clubRepository.GetById(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}
