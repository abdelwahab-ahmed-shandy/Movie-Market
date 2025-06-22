
namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class PopularCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public PopularCategoriesViewComponent(ICategoryService categoryService)
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
