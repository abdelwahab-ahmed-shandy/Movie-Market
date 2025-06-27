using DAL.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    [Route("Identity/Settings/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _profileService = profileService;

        }

        #region Manage Profile

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            

            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var model = await _profileService.GetProfileAsync(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _profileService.UpdateProfileAsync(model);
                TempData["notification"] = "Profile updated successfully!";
                TempData["MessageType"] = "success"; 

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        #endregion


        #region View profile
        [HttpGet("Details")]
        public async Task<IActionResult> Details()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var model = await _profileService.GetProfileAsync(userId);
            return View(model);
        }
        #endregion

    }
}
