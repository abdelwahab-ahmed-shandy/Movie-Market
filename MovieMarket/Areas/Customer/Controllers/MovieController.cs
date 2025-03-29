using Microsoft.AspNetCore.Mvc;
using MovieMart.Repositories;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository movieRepository;

        public MovieController(IMovieRepository movieRepository)
        {
            this.movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            var movies = movieRepository.Get()
                .Include(c => c.Category)
                .ToList();
            return View(movies);
        }

        public IActionResult MoreDetils(int Id)
        {

            var ViewMovie = movieRepository.Get()
                .Include(M => M.Category)
                .FirstOrDefault(M => M.Id == Id);

            if (ViewMovie == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(ViewMovie);
        }
    }
}
