using Microsoft.AspNetCore.Mvc;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class TvSerieController : Controller
    {
        private readonly ITvSeriesRepository tvSeriesRepository;
        public TvSerieController(ITvSeriesRepository tvSeriesRepository)
        {
            this.tvSeriesRepository = tvSeriesRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tvSerie = tvSeriesRepository.Get().ToList();
            return View(tvSerie);
        }


    }
}
