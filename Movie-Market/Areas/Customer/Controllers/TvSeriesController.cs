using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TvSeriesController : Controller
    {
        private readonly ICustomerTvSeriesService _tvSeriesService;

        public TvSeriesController(ICustomerTvSeriesService tvSeriesService)
        {
            _tvSeriesService = tvSeriesService;
        }


        public async Task<IActionResult> Index()
        {
            var tvSeries = await _tvSeriesService.GetAllTvSeriesAsync();
            return View(tvSeries);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var tvSeriesDetails = await _tvSeriesService.GetTvSeriesWithDetailsAsync(id);

            if (tvSeriesDetails == null)
            {
                return NotFound();
            }

            return View(tvSeriesDetails);
        }



    }
}
