using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMarket.Repositories.IRepositories;
using MovieMart.Repositories;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository _cinemaRepository;
        public MovieController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository)
        {
            this.movieRepository = movieRepository;
            this._cinemaRepository = cinemaRepository;
        }

        public IActionResult Index()
        {
            var movies = movieRepository.Get()
                .Include(c => c.Category)
                .ToList();
            return View(movies);
        }

        public async Task<IActionResult> MoreDetils(int Id)
        {

            var ViewMovie = movieRepository.Get()
                .Include(M => M.Category)
                .Include(m => m.CinemaMovies)
                    .ThenInclude(cm => cm.Cinema)
                .FirstOrDefault(M => M.Id == Id);

            if (ViewMovie == null)
                return NotFound();

            var cinemas = ViewMovie.CinemaMovies
                        .Select(cm => cm.Cinema)
                        .ToList();

            ViewBag.Cinemas = cinemas;

            return View(ViewMovie);
        }
    }
}
