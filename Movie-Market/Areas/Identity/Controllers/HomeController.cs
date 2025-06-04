using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
