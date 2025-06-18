using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SpecialController : BaseController
    {
        private readonly ICustomerSpecialService _customerSpecialService;

        public SpecialController(ICustomerSpecialService customerSpecialService)
        {
            _customerSpecialService = customerSpecialService;
        }


        // GET: Customer/Specials
        public async Task<IActionResult> Index()
        {
            var specials = await _customerSpecialService.GetActiveSpecialAsync();
            return View(specials);
        }

        // GET: Customer/Specials/Details/5
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
