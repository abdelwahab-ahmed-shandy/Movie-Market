using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMart.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MovieMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult Index(string? query, int page = 1)
        {
            var category = _categoryRepository.Get().ToList();


            // Check if there is a search query
            if (query != null)
            {
                category = category.Where(e => (e.Name ?? "").Contains(query) // Search for the Name
                || (e.Description ?? "").Contains(query)) // Search for the Description
                .ToList();
            }

            // Calculate the number of pages required, so that there are 5 customers per page
            int pageSize = 5;
            int totalCustomers = category.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            // Check if the requested page does not exist
            if (page > totalPages && totalPages > 0)
                return NotFound();

            // Split the customers and display only 5 per page
            category = category.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the number of pages to the View
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            return View(category);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Create(category);
                _categoryRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "The category was created successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            // Set the error message in case of a problem
            TempData["Message"] = "An error occurred while creating the class!";
            TempData["MessageType"] = "error";

            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == Id);

            if (category != null)
            {
                return View(category);
            }
            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category == null || !ModelState.IsValid)
            {
                TempData["notifiction"] = "Category not found!";
                TempData["MessageType"] = "error";

                return RedirectToAction("NotFound", "Home");

            }
            _categoryRepository.Edit(category);
            _categoryRepository.SaveDB();

            TempData["notifiction"] = "Edit Category Successfully!";
            TempData["MessageType"] = "Success";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int Id)
        {
            var category = _categoryRepository.GetOne(c => c.Id == Id);

            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _categoryRepository.Delete(category);
            _categoryRepository.SaveDB();

            TempData["notifiction"] = "Category Deleted Successfully!";
            TempData["MessageType"] = "Success";
            return RedirectToAction(nameof(Index));
        }

    }
}
