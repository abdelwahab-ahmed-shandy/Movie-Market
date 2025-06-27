using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CharacterController : BaseController
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _characterService = characterService;
        }

        public async Task<IActionResult> Index()
        {
            var characters = await _characterService.GetAllCharactersAsync();
            return View(characters);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var character = await _characterService.GetCharacterDetailsAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return View(character);
        }
     
    }
}
