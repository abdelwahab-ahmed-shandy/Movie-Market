using Microsoft.AspNetCore.Mvc;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class CharacterController : Controller
    {
        private readonly ICharacterRepository characterRepository;
        public CharacterController(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var character = characterRepository.Get();
            return View(character);
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var characterDetails = characterRepository.Get()
                .Include(m => m.CharacterMovies)
                .ThenInclude(cm => cm.Movie)
                .Include(t => t.CharacterTvSeries)
                .ThenInclude(cts => cts.TvSeries)
                .FirstOrDefault(C => C.Id == Id);

            if (characterDetails == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(characterDetails);
        }
    }
}
