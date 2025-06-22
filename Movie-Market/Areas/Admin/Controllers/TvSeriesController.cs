using DAL.ViewModels.TvSeries;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class TvSeriesController : BaseController
    {
        private readonly ITvSeriesService _tvSeriesService;
        private readonly ILogger<TvSeriesController> _logger;
        public TvSeriesController(ITvSeriesService tvSeriesService, ILogger<TvSeriesController> logger)
        {
            _tvSeriesService = tvSeriesService;
            _logger = logger;
        }


        #region TvSeries Views 

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string query = null)
        {
            var model = await _tvSeriesService.GetAllTvSeriesAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var tvSeries = await _tvSeriesService.GetTvSeriesDetailsAsync(id);
                return View(tvSeries);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting TV series details");
                TempData["ErrorMessage"] = "An error occurred while retrieving TV series details.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion


        #region Create 

        [HttpGet]
        public IActionResult Create()
        {
            var model = new TvSeriesAdminCreateVM
            {
                CurrentState = CurrentState.Active 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TvSeriesAdminCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tvSeriesService.CreateTvSeriesAsync(model);

                    TempData["notification"] = "TV Series created successfully!";
                    TempData["MessageType"] = "success";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating TV Series: {ex.Message}");
                }
            }
            return View(model);
        }
        #endregion


        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var details = await _tvSeriesService.GetTvSeriesDetailsAsync(id);
            if (details == null)
            {
                return NotFound();
            }

            var model = new TvSeriesAdminEditVM
            {
                Id = details.Id,
                Title = details.Title,
                Description = details.Description,
                Author = details.Author,
                ReleaseYear = details.ReleaseYear,
                CurrentState = details.IsDeleted ? CurrentState.SoftDelete : CurrentState.Active
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] TvSeriesAdminEditVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tvSeriesService.UpdateTvSeriesAsync(id, model);

                    TempData["notification"] = "TV Series updated successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating TV Series: {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion


        #region Delete 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return NotFound();

                await _tvSeriesService.Delete(id);

                TempData["notification"] = "TV Series permanently deleted!";
                TempData["MessageType"] = "Warning";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting TV Series: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return NotFound();

                await _tvSeriesService.SoftDelete(id);

                TempData["notification"] = "TV Series soft deleted!";
                TempData["MessageType"] = "Information";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error soft deleting TV Series: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return NotFound();

                await _tvSeriesService.RestoreAsync(id);

                TempData["notification"] = "TV Series restored successfully!";
                TempData["MessageType"] = "Warning";

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error restoring TV Series: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion


    }
}
