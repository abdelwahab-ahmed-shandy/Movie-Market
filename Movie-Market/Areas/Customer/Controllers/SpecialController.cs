using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SpecialController : BaseController
    {
        private readonly ISpecialService _customerSpecialService;

        public SpecialController(ISpecialService customerSpecialService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _customerSpecialService = customerSpecialService;
        }


        public async Task<IActionResult> Index()
        {
            var specials = await _customerSpecialService.GetActiveSpecialAsync();
            return View(specials);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var special = await _customerSpecialService.GetSpecialDetailsAsync(id);
            if (special == null)
            {
                return NotFound();
            }

            return View(special);
        }


    }
}
