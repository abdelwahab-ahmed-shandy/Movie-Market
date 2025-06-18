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
        private readonly IAdminDashboardService _dashboardService;
        public HomeController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        #region Admin DashBord
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData);
        }
        #endregion

    }
}
