using BLL.Services.Interfaces.Customer;

namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class PopularCategoriesViewComponent : ViewComponent
    {
        private readonly ICustomerCategoryService _categoryService;

        public PopularCategoriesViewComponent(ICustomerCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var categories = await _categoryService.GetPopularCategoriesAsync(count);
            return View("_PopularCategories", categories);
        }
    }
}
