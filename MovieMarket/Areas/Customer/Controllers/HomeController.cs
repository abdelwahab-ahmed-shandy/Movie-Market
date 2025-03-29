using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IMovieRepository movieRepository;

        public HomeController(IMovieRepository movieRepository)
        {
            this.movieRepository = movieRepository;
        }

        [HttpGet]
        public IActionResult Index(Movie movie)
        {
            var movies = movieRepository.Get()
                .Include(c => c.Category)
                .ToList();
            return View(movies);
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult Abdelwahab()
        {

            return View();
        }

        //TODO:here Not found page and Errors And Page With you
    }
}
