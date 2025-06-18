using BLL.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoriesController : BaseController
    {
        private readonly ICustomerCategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICustomerCategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryWithMoviesAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching category details for id: {id}");
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        public async Task<IActionResult> PopularCategoriesPartial(int count = 5)
        {
            try
            {
                var categories = await _categoryService.GetPopularCategoriesAsync(count);
                return PartialView("_PopularCategories", categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching popular categories");
                return PartialView("_ErrorPartial", "An error occurred while fetching popular categories");
            }
        }


    }
}

