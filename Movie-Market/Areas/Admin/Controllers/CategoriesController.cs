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
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var model = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize, searchTerm);
            return View(model);
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

                TempData["notification"] = "Category created successfully!";
                TempData["MessageType"] = "success";


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


            return View("Create", VM);
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

                TempData["notification"] = "Category Is Updated !";
                TempData["MessageType"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            return View("Create", VM);
        }

        #endregion


        #region Delete 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            await _categoryService.ToggleCategoryStatusAsync(id);

            TempData["notification"] = "Category Change Status Soft Deleted!";
            TempData["MessageType"] = "Information";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            var category = await _categoryService.GetCategoryDetailsAsync(id);
            if (category == null)
                return NotFound();

            await _categoryService.DeleteCategoryPermanentlyAsync(id);

            TempData["notification"] = "Category permanently deleted!";
            TempData["MessageType"] = "danger";

            return RedirectToAction(nameof(Index));
        }

        #endregion

    }
}
