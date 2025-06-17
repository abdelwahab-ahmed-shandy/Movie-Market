using DAL.ViewModels.Episode;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class EpisodeController : Controller
    {
        private readonly IAdminEpisodeService _episodeService;
        public EpisodeController(IAdminEpisodeService episodeService)
        {
            _episodeService = episodeService;
        }

        #region Views
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, Guid seasonId = default, string? query = null)
        {
            var model = await _episodeService.GetAllEpisodesAsync(page, pageSize, seasonId, query);
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var episode = await _episodeService.GetEpisodeDetailsAsync(id);
            return View(episode);
        }
        #endregion


        #region Create

        public IActionResult Create(Guid seasonId)
        {
            var model = new EpisodeAdminCreateVM { SeasonId = seasonId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EpisodeAdminCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _episodeService.CreateEpisodeAsync(model);
                    TempData["SuccessMessage"] = "Episode created successfully!";
                    return RedirectToAction(nameof(Index), new { seasonId = model.SeasonId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating episode: {ex.Message}");
                }
            }
            return View(model);
        }
        
        #endregion


        #region Edit 

        public async Task<IActionResult> Edit(Guid id)
        {
            var details = await _episodeService.GetEpisodeDetailsAsync(id);
            var model = new EpisodeAdminEditVM
            {
                Id = details.Id,
                EpisodeNumber = details.EpisodeNumber,
                Title = details.Title,
                Description = details.Description,
                Rating = details.Rating,
                Duration = details.Duration,
                VideoUrl = details.VideoUrl,
                ThumbnailUrl = details.ThumbnailUrl,
                CurrentState = details.CurrentState
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EpisodeAdminEditVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _episodeService.UpdateEpisodeAsync(id, model);
                    TempData["SuccessMessage"] = "Episode updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating episode: {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion


        #region

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                await _episodeService.SoftDeleteEpisodeAsync(id);
                TempData["SuccessMessage"] = "Episode soft deleted!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error soft deleting episode: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _episodeService.RestoreEpisodeAsync(id);
                TempData["SuccessMessage"] = "Episode restored!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error restoring episode: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _episodeService.DeleteEpisodeAsync(id);
                TempData["SuccessMessage"] = "Episode permanently deleted!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting episode: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion


    }
}
