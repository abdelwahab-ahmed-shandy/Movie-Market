
namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class CinemaController : Controller
    {
        private readonly IAdminCinemaService _cinemaService;

        public CinemaController(IAdminCinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }


        #region Cinema Index

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var cinemas = await _cinemaService.GetAllCinemasAsync(pageNumber, pageSize, searchTerm);
            return View(cinemas);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var cinema = await _cinemaService.GetCinemaDetailsAsync(id);

            if (cinema == null)
                return NotFound();

            return View(cinema);
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
        public async Task<IActionResult> Create(CinemaAdminVM VM)
        {
            if (ModelState.IsValid)
            {
                await _cinemaService.CreateAsync(VM);

                TempData["notification"] = "Cinema Created Successfully!";
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
            var cinema = await _cinemaService.GetByIdAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CinemaAdminVM VM)
        {
            if (id != VM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _cinemaService.UpdateAsync(VM);

                TempData["notification"] = "Cinema Updated Successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            return View(VM);
        }

        #endregion



        #region Delete 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            await _cinemaService.SoftDeleteAsync(id);

            TempData["notification"] = "Cinema soft-deleted or restored successfully!";
            TempData["MessageType"] = "Information";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            await _cinemaService.DeleteAsync(id);

            TempData["notification"] = "Cinema permanently deleted!";
            TempData["MessageType"] = "Warning";

            return RedirectToAction(nameof(Index));
        }

        #endregion


    }
}
