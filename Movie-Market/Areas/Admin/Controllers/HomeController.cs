using DAL.ViewModels.Dashboard;
using DAL.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class HomeController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        #region Admin DashBord
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetAdminDashboardDataAsync();
            return View(dashboardData);
        }
        #endregion

    }
}
