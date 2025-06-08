using DAL.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();


        }


    }
}
