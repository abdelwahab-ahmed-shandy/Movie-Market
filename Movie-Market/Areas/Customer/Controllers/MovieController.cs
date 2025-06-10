using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly ICustomerMovieService _movieService;

        public MovieController(ICustomerMovieService movieService)
        {
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetActiveMoviesAsync();
            return View(movies);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var movie = await _movieService.GetMovieDetailsAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public async Task<IActionResult> Popular()
        {
            var popularMovies = await _movieService.GetPopularMoviesAsync(5);
            return View(nameof(Index), popularMovies);
        }

        public async Task<IActionResult> ByCategory(Guid categoryId)
        {
            var movies = await _movieService.GetMoviesByCategoryAsync(categoryId);
            return View(nameof(Index), movies);
        }


    }
}
