using DAL.Context;

namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class CategoriesDropdownViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoriesDropdownViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            return View(categories);
        }
    }
}
