using BLL.Services.Interfaces.Customer;
using DAL.ViewModels.Dashboard;
using DAL.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IDashboardService _dashboardService;
        private readonly ISearchService _searchService;
        private readonly ICustomerMovieService _movieService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ISearchService searchService , ICustomerMovieService movieService,
                                 ILogger<HomeController> logger, IDashboardService dashboardService , IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _searchService = searchService;
            _logger = logger;
            _movieService = movieService;
            _dashboardService = dashboardService;
        }


        #region index Customer Home Page

        public async Task<IActionResult> Index() 
        {
            const string cacheKey = "DashboardData";

            if (!_cache.TryGetValue(cacheKey, out DashboardVM data))
            {
                data = await _dashboardService.GetDashboardDataAsync();
                _cache.Set(cacheKey, data, TimeSpan.FromMinutes(10));
            }

            return View(data); 
        }

        #endregion


        #region AS Global Search
        // For full page search results
        [HttpGet]
        public async Task<IActionResult> GlobalSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new SearchCutomerResultVM());

            var results = await _searchService.GlobalSearchCustomerAsync(query);
            return View(results);
        }

        // For AJAX search dropdown
        [HttpGet]
        public async Task<IActionResult> GlobalSearchPartial(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return PartialView("_SearchResults", new SearchCutomerResultVM());

            var results = await _searchService.GlobalSearchCustomerAsync(query);
            return PartialView("_SearchResults", results);
        }
        #endregion


        #region New Releases

        [HttpGet]
        public async Task<IActionResult> NewReleases()
        {
            try
            {
                var newReleases = await _movieService.GetMoviesNewReleasesAsync(20);
                return View(newReleases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading new releases");
                return View("Error");
            }
        }

        #endregion


    }
}
