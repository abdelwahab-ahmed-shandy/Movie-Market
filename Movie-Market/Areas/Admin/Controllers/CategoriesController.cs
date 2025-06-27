using DAL.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoriesController : BaseController
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoryService _categoryService;
        public CategoriesController(ILogger<CategoriesController> logger,
                                     ICategoryService categoryService, UserManager<ApplicationUser> userManager) : base(userManager)
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
        public async Task<IActionResult> Create([FromForm] CategoryAdminCreateEditVM VM)
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
                CurrentState = category.CurrentState,
            };


            return View("Create", VM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] CategoryAdminCreateEditVM VM)
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
                TempData["MessageType"] = "Success";
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
            try
            {
                await _categoryService.SoftDelete(id);

                TempData["notification"] = "Category status changed successfully!";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                TempData["notification"] = ex.Message;
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["notification"] = $"Error changing status: {ex.Message}";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryDetailsAsync(id);
                if (category == null)
                {
                    TempData["notification"] = "Category not found";
                    TempData["MessageType"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                await _categoryService.DeleteAsync(id);

                TempData["notification"] = $"Category '{category.Name}' was permanently deleted!";
                TempData["MessageType"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                TempData["notification"] = ex.Message;
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["notification"] = $"Error deleting category: {ex.Message}";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

    }
}
