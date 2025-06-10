
namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CinemaController : Controller
    {
        private readonly IAdminCinemaService _cinemaService;

        public CinemaController(IAdminCinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }


        #region Index

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _cinemaService.GetAllAsync();
            return View(cinemas);
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
            await _cinemaService.SoftDeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _cinemaService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        #endregion


    }
}
