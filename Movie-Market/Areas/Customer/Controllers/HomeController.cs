using DAL.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ISearchService _searchService;

        public HomeController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        public IActionResult Index()
        {
            return View();
        }


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




    }
}
