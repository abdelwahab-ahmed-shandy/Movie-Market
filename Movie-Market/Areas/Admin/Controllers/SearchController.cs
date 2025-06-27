using DAL.ViewModels.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SearchController : BaseController
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new SearchAdminResultVM());

            var results = await _searchService.GlobalSearchAdminAsync(query);
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> GlobalSearchPartial(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return PartialView("_AdminSearchResults", new SearchAdminResultVM());

            var results = await _searchService.GlobalSearchAdminAsync(query);
            return PartialView("_AdminSearchResults", results);
        }


    }
}
