using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : BaseController
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService, UserManager<ApplicationUser> userManager) : base(userManager)
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
            var movie = await _movieService.GetMovieWithDetailsAsync(id);
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
