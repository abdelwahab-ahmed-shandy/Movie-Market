using Microsoft.AspNetCore.Mvc;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class EpisodeController : Controller
    {
        private readonly IEpisodeRepository episodeRepository;

        public EpisodeController(IEpisodeRepository episodeRepository)
        {
            this.episodeRepository = episodeRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var episode = episodeRepository.Get()
                .Include(S => S.Season);
            return View(episode.ToList());
        }

        [HttpGet]
        public IActionResult AllEpisode(int Id)
        {
            var episode = episodeRepository.Get()
                .Where(e => e.SeasonId == Id)
                .Include(S => S.Season)
                .ToList();



            return View(episode);
        }
    }
}
