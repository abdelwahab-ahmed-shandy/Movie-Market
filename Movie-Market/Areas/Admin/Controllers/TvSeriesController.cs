using DAL.ViewModels.TvSeries;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class TvSeriesController : Controller
    {
        private readonly IAdminTvSeriesService _tvSeriesService;
        private readonly ILogger<TvSeriesController> _logger;
        public TvSeriesController(IAdminTvSeriesService tvSeriesService, ILogger<TvSeriesController> logger)
        {
            _tvSeriesService = tvSeriesService;
            _logger = logger;
        }

        #region TvSeries Views 
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string query = null)
        {
            var model = await _tvSeriesService.GetAllTvSeriesAsync(page, pageSize, query);
            return View(model);
        }

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


        #region Create :
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
        public async Task<IActionResult> Create(TvSeriesAdminCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tvSeriesService.CreateTvSeriesAsync(model);
                    //TempData["SuccessMessage"] = "TV Series created successfully!";
                    //TempData["Ma"];
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
                ImgUrl = details.ImgUrl,
                ReleaseYear = details.ReleaseYear,
                CurrentState = details.IsDeleted ? CurrentState.SoftDelete : CurrentState.Active
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TvSeriesAdminEditVM model)
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
                    TempData["SuccessMessage"] = "TV Series updated successfully!";
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
                await _tvSeriesService.Delete(id);
                TempData["SuccessMessage"] = "TV Series permanently deleted!";
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
                await _tvSeriesService.SoftDelete(id);
                TempData["SuccessMessage"] = "TV Series soft deleted!";
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
                // Note: You'll need to add RestoreAsync to your IAdminTvSeriesService
                await _tvSeriesService.RestoreAsync(id);
                TempData["SuccessMessage"] = "TV Series restored successfully!";
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
