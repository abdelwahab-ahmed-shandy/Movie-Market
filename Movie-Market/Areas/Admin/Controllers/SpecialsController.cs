
namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class SpecialsController : Controller
    {
        private readonly IAdminSpecialService _specialService;

        public SpecialsController(IAdminSpecialService specialService)
        {
            _specialService = specialService;
        }


        #region Special Views 

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var specials = await _specialService.GetAllSpecialsAsync(pageNumber, pageSize, searchTerm);
            return View(specials);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var special = await _specialService.GetSpecialByIdAsync(id);
            if (special == null) return NotFound();
            return View(special);
        }

        #endregion



        #region Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var id = await _specialService.CreateSpecialAsync(model);

                TempData["notification"] = "Special created successfully!";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(model);
        }

        #endregion



        #region Edit

        public async Task<IActionResult> Edit(Guid id)
        {
            var special = await _specialService.GetSpecialByIdAsync(id);
            if (special == null) return NotFound();

            var model = new SpecialEditVM
            {
                Id = special.Id,
                Name = special.Name,
                Description = special.Description,
                DiscountPercentage = special.DiscountPercentage,
                StartDate = special.StartDate,
                EndDate = special.EndDate,
                IsActive = special.CurrentState == CurrentState.Active
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SpecialEditVM model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _specialService.UpdateSpecialAsync(id, model);

                TempData["notification"] = "Special Is Updated !";
                TempData["MessageType"] = "Success";
                if (success) return RedirectToAction(nameof(Details), new { id });
            }
            return View(model);
        }

        #endregion



        #region Delete 

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var success = await _specialService.ToggleSpecialStatusAsync(id);
            return Json(new { success });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _specialService.DeleteAsync(id);
            TempData["notification"] = "Special Deleted successfully!";
            TempData["MessageType"] = "Warning";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _specialService.SoftDeleteAsync(id);

            TempData["notification"] = "Special Soft Deleted successfully!";
            TempData["MessageType"] = "Information";
            return RedirectToAction(nameof(Index));

        }
        #endregion


    }
}