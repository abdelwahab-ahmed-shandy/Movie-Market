using DAL.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly IAdminCategoryService _categoryService;
        public CategoriesController(ILogger<CategoriesController> logger,
                                     IAdminCategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        #region View Category

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var category = await _categoryService.GetCategoryDetailsAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        #endregion


        #region Create 

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAdminCreateEditVM VM)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(VM);
                return RedirectToAction(nameof(Index));
            }

            return View(VM);
        }

        #endregion

        #region Edit 

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await _categoryService.GetCategoryDetailsAsync(id);
            if (category == null)
                return NotFound();

            var VM = new CategoryAdminCreateEditVM
            {
                Name = category.Name,
                Description = category.Description,
                CurrentState = category.CurrentState
            };

            return View(VM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryAdminCreateEditVM VM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateCategoryAsync(id, VM);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(VM);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(VM);
        }

        #endregion


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            await _categoryService.ToggleCategoryStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
