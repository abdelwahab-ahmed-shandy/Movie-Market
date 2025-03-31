using Microsoft.AspNetCore.Mvc;
using MovieMarket.Repositories.IRepositories;
using MovieMart.Repositories;
using MovieMart.Repositories.IRepositories;

namespace MovieMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;

        public CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository)
        {
            this._cinemaRepository = cinemaRepository;
            this._movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            var ViewCinema = _cinemaRepository.Get().ToList();

            return View(ViewCinema);
        }

        public IActionResult MoreMovieWithCinema(int Id)
        {
            var cinemaWithMovies = _cinemaRepository.Get()
        .Where(c => c.Id == Id)
        .Include(c => c.CinemaMovies)
        .ThenInclude(cm => cm.Movie) // Include movies related to the cinema
        .ThenInclude(m => m.Category) // Include category for each movie
        .FirstOrDefault();

            if (cinemaWithMovies == null || !cinemaWithMovies.CinemaMovies.Any())
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(cinemaWithMovies); // Pass the entire Cinema object with related movies
        }
    }
}
