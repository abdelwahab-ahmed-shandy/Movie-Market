using DAL.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class UpdateController : BaseController
    {
        private readonly IProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UpdateController(IProfileService profileService, UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager) : base(userManager)
        {
            _profileService = profileService;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        #region Change Password

        [HttpGet]
        public IActionResult ChangePassword()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["notification"] = "Invalid data";
                TempData["MessageType"] = "Error";
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["notification"] = "User not found";
                TempData["MessageType"] = "Error";

                return RedirectToAction("Index", "Home" , new {area = "Customer"});
            }

            var result = await _profileService.ChangePasswordAsync(Guid.Parse(userId), model);

            if (result == null || !result.Succeeded)
            {
                TempData["notification"] = "Failed to change password";
                TempData["MessageType"] = "Error";

                if (result?.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                return View(model);
            }

            TempData["notification"] = "Password changed successfully";
            TempData["MessageType"] = "Success";
            return RedirectToAction("Details", "Profile", new { area = "Identity" });
        }
        #endregion


        #region Delete Account

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteAccount()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["notification"] = "User not found";
                TempData["MessageType"] = "Error";
                return RedirectToAction("Details", "Profile", new { area = "Identity" });
            }

            var result = await _profileService.DeleteAccountAsync(Guid.Parse(userId));

            if (result?.Succeeded == true)
            {
                await _signInManager.SignOutAsync();

                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                return RedirectToAction("Register", "Account");
            }

            TempData["notification"] = result?.Errors?.FirstOrDefault()?.Description ?? "Account deletion failed. Please try again later.";
            TempData["MessageType"] = "Error";
            return RedirectToAction("Details", "Profile", new { area = "Identity" });
        }

        #endregion



    }
}
