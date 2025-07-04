﻿using DAL.ViewModels.Season;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SeasonController : BaseController
    {
        private readonly ISeasonService _seasonService;

        public SeasonController(ISeasonService seasonService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _seasonService = seasonService;
        }


        #region Views

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, Guid tvSeriesId = default, string? query = null)
        {
            var model = await _seasonService.GetAllSeasonsAsync(page, pageSize, tvSeriesId, query);
            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var season = await _seasonService.GetSeasonDetailsAsync(id);
            return View(season);
        }

        #endregion


        #region Create 

        public IActionResult Create(Guid tvSeriesId)
        {
            var model = new SeasonAdminCreateVM { TvSeriesId = tvSeriesId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeasonAdminCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _seasonService.CreateSeasonAsync(model);
                    TempData["SuccessMessage"] = 

                    TempData["notification"] = "Season created successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction(nameof(Index), new { tvSeriesId = model.TvSeriesId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating season: {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion


        #region Edit

        public async Task<IActionResult> Edit(Guid id)
        {
            var details = await _seasonService.GetSeasonDetailsAsync(id);
            var model = new SeasonAdminEditVM
            {
                Id = details.Id,
                Title = details.Title,
                SeasonNumber = details.SeasonNumber,
                ReleaseYear = details.ReleaseYear,
                CurrentState = details.CurrentState
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SeasonAdminEditVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _seasonService.UpdateSeasonAsync(id, model);

                    TempData["notification"] = "Season updated successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating season: {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion


        #region Delete


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                await _seasonService.SoftDeleteSeasonAsync(id);

                TempData["notification"] = "Season soft deleted!";
                TempData["MessageType"] = "Information";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error soft deleting season: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _seasonService.RestoreSeasonAsync(id);

                TempData["notification"] = "Season restored!";
                TempData["MessageType"] = "Information";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error restoring season: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _seasonService.DeleteSeasonAsync(id);

                TempData["notification"] = "Season permanently deleted!";
                TempData["MessageType"] = "Warning";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting season: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }


        #endregion


    }
}
