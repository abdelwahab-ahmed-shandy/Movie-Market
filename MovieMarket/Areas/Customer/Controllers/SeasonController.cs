using Microsoft.AspNetCore.Mvc;
using MovieMart.Repositories;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository seasonRepository;
        public SeasonController(ISeasonRepository seasonRepository)
        {
            this.seasonRepository = seasonRepository;
        }
        public IActionResult Index()
        {
            var Season = seasonRepository.Get()
                .Include(S => S.TvSeries);
            return View(Season.ToList());
        }

        [HttpGet]
        public IActionResult AllSeasons(int Id)
        {
            var tvSerie = seasonRepository.Get()
                .Where(S => S.TvSeriesId == Id)
                .ToList();
            return View(tvSerie);
        }
    }
}
