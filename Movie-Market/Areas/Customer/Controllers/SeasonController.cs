using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SeasonController : BaseController
    {
        private readonly ISeasonService _seasonService;
        public SeasonController(ISeasonService seasonService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _seasonService = seasonService;
        }

        public async Task<IActionResult> Index(Guid tvSeriesId)
        {
            var seasons = await _seasonService.GetAllSeasonAsync();
            seasons = seasons.Where(s => s.Id == tvSeriesId).ToList();
            ViewBag.TvSeriesId = tvSeriesId;
            return View(seasons);
        }
         
        public async Task<IActionResult> Details(Guid id)
        {
            var season = await _seasonService.GetSeasonWithDetailsAsync(id);
            if (season == null)
            {
                return NotFound();
            }
            return View(season);
        }


    }
}
