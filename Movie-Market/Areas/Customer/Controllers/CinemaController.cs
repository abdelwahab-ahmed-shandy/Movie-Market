using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        private readonly ICustomerCinemaService _cinemaService;

        public CinemaController(ICustomerCinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaService.GetActiveCinemaAsync();
            return View(cinemas);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var cinema = await _cinemaService.GetCinemaDetailsAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        public async Task<IActionResult> Popular()
        {
            var popularCinemas = await _cinemaService.GetPopularCinemaAsync(5);
            return View(popularCinemas);
        }

    }
}
