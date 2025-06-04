using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
