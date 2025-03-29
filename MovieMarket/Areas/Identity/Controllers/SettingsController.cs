using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieMarket.Models.ViewModels;
using MovieMarket.Repositories.IRepositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieMarket.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public SettingsController(UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                  SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        #region Profile

        // Get profile data for the currently logged-in user
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Get the current user's data through the UserManager
            var user = await _userManager.GetUserAsync(User);

            // If the user is not found, they are redirected to the "Not Found" page
            if (user == null)
            {
                return RedirectToAction("NotFound", "Account", new { area = "Identity" });
            }

            // Set the page title in ViewData to display in the UI
            ViewData["Title"] = "Profile";

            // Create a ViewModel object containing user data to display in the UI
            var profile = new ProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                UserName = user.UserName
            };

            // Display the profile page and send data to it
            return View(profile);
        }

        #endregion


        #region Manage Profile

        // Display the profile edit page
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            // Get the current user's data
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Redirect the user to the "Not Found" page if they are not found
                return RedirectToAction("NotFound", "Account", new { area = "Identity" });
            }

            // Fill in the profile edit form with the current user's data
            var profile = new ManageProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
            };
            return View(profile);
        }


        // Processing profile modifications after form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Protection against CSRF attacks
        public async Task<IActionResult> Manage(ManageProfileVM manageProfileVM)
        {
            // Validate the entered data
            if (!ModelState.IsValid)
            {
                return View(manageProfileVM);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Redirect to the "Not Found" page if the user is not found
                return RedirectToAction("NotFound", "Account", new { area = "Identity" });
            }

            // Check if the user entered the current password
            if (string.IsNullOrEmpty(manageProfileVM.CurrentPassword))
            {
                ModelState.AddModelError("", "You must enter the current password to update the profile.");
                return View(manageProfileVM);
            }

            // Check the current password
            var userPassValid = await _userManager.CheckPasswordAsync(user, manageProfileVM.CurrentPassword);
            if (!userPassValid)
            {
                ModelState.AddModelError("", "The current password is incorrect.");
                return View(manageProfileVM);
            }

            // Update basic user data
            user.FirstName = manageProfileVM.FirstName;
            user.LastName = manageProfileVM.LastName;
            user.Address = manageProfileVM.Address;
            user.UserName = manageProfileVM.UserName;
            user.Email = manageProfileVM.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(manageProfileVM);
            }

            // Change the password if a new password is entered
            if (!string.IsNullOrEmpty(manageProfileVM.NewPassword) && !string.IsNullOrEmpty(manageProfileVM.ConfirmNewPassword))
            {
                // Check if the new password and confirmation match
                if (manageProfileVM.NewPassword != manageProfileVM.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "The new password and confirmation do not match.");
                    return View(manageProfileVM);
                }

                // Perform the password change operation
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, manageProfileVM.CurrentPassword, manageProfileVM.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(manageProfileVM);
                }
            }

            TempData["notifiction"] = "Update You Account Successfully!";
            TempData["MessageType"] = "Success";
            // Redirect the user to the profile page after a successful update
            return RedirectToAction("Profile", "Settings", new { area = "Identity" });
        }

        #endregion

        #region Delete Customer (No one uses the delete button except the account owner.)

        // Displays the account deletion confirmation page when requested via HTTP GET
        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }

        // Processes the account deletion request when sent via HTTP POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF (Cross-Site Request Forgery) attacks
        public async Task<IActionResult> ConfirmDeleteAccount()
        {
            // Gets the current user via UserManager
            var user = await _userManager.GetUserAsync(User);

            // If the user is not found, an error message is displayed and redirects to the home page
            if (user == null)
            {
                TempData["notifiction"] = "User not found.";
                TempData["MessageType"] = "Error";
                return RedirectToAction("Index", "Home");
            }

            // Checks if the user has the "Admin" or "SuperAdmin" role
            if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                // Prevents account deletion if the user is an administrator (Admin or SuperAdmin)
                TempData["notifiction"] = "Admins and SuperAdmins cannot delete their own accounts.";
                TempData["MessageType"] = "Error";
                return RedirectToAction("Profile");
            }

            // Updates the Security Stamp to ensure session security before deleting the account
            await _userManager.UpdateSecurityStampAsync(user);

            // Deletes the account via the UserManager
            var result = await _userManager.DeleteAsync(user);

            // If the deletion fails, an error message is displayed and the user is redirected to the profile page.
            if (!result.Succeeded)
            {

                // Set the success message in TempData
                TempData["notifiction"] = "An error occurred while deleting the account.";
                TempData["MessageType"] = "error";

                return RedirectToAction("Profile");
            }

            // Signs the user out after deleting their account.

            await _signInManager.SignOutAsync();

            // Displays a success message and redirects the user to the "Customer" section home page.
            TempData["notifiction"] = "Your account has been successfully deleted.";
            TempData["MessageType"] = "Warning";

            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        #endregion

    }
}
