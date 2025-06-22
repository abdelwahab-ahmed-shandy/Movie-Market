using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : BaseController
    {
        private readonly ICinemaService _cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaService.GetActiveCinemaAsync();
            return View(cinemas);
        }

        public async Task<IActionResult> Details(Guid id, string searchString, string sortOrder)
        {
            var cinemaDetails = await _cinemaService.GetCinemaDetailsAsync(id, searchString, sortOrder);

            if (cinemaDetails == null)
            {
                return NotFound();
            }

            return View(cinemaDetails);
        }

        public async Task<IActionResult> Popular()
        {
            var popularCinemas = await _cinemaService.GetPopularCinemaAsync(5);
            return View(popularCinemas);
        }

    }
}
