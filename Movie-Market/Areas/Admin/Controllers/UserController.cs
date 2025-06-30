using DAL.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }


        public async Task<IActionResult> Admins()
        {
            var admins = await _userService.GetAdminsAsync();
            ViewData["Title"] = "Admins Management";
            ViewData["UserType"] = "Admins";
            return View("Index", admins);
        }


        public async Task<IActionResult> SuperAdmins()
        {
            var superAdmins = await _userService.GetSuperAdminsAsync();
            ViewData["Title"] = "Super Admins Management";
            ViewData["UserType"] = "Super Admins";
            return View("Index", superAdmins);
        }


        public async Task<IActionResult> Customers()
        {
            var customers = await _userService.GetCustomersAsync();
            ViewData["Title"] = "Customers Management";
            ViewData["UserType"] = "Customers";
            return View("Index", customers);
        }


        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userService.GetUserDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BlockUserPolicy")]
        public async Task<IActionResult> BlockUser(BlockUserVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.BlockUserAsync(
                        model.UserId,
                        model.BlockReason,
                        User.Identity.Name);

                    return RedirectToAction(nameof(Details), new { id = model.UserId });
                }

                var user = await _userService.GetUserDetailsAsync(model.UserId);
                if (user == null) return NotFound();

                user.NewBlockReason = model.BlockReason;
                return View("Details", user);
            }
            catch (InvalidOperationException ex)
            {

                TempData["notification"] = ex.Message;
                TempData["MessageType"] = "error";

                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["notification"] = ex.Message;
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser(Guid userId)
        {
            await _userService.UnblockUserAsync(userId, User.Identity.Name);
            return RedirectToAction(nameof(Details), new { id = userId });
        }


        [HttpGet]
        public async Task<IActionResult> ChangeRole(Guid id)
        {
            var model = await _userService.GetUserRoleInfoAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeUserRoleVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.ChangeUserRoleAsync(model, User.Identity.Name);

                    TempData["notification"] = "User role updated successfully";
                    TempData["MessageType"] = "Success";

                    return RedirectToAction(nameof(Details), new { id = model.UserId });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["notification"] = ex.Message;
                TempData["MessageType"] = "Error";

                return RedirectToAction(nameof(Details), new { id = model.UserId });
            }
        }

    }
}
