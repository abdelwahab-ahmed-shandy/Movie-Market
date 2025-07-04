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


        public async Task<IActionResult> Index(int page = 1, string search = null)
        {
            var users = await _userService.GetAllUsersAsync(page, search);
            ViewData["Search"] = search;
            return View(users);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Admins(int page = 1, string search = null)
        {
            var admins = await _userService.GetAdminsAsync(page, 10, search);
            ViewData["Title"] = "Admins Management";
            ViewData["UserType"] = "Admins";
            ViewData["Search"] = search;
            return View("Index", admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SuperAdmins(int page = 1, string search = null)
        {
            var superAdmins = await _userService.GetSuperAdminsAsync(page, 10, search);
            ViewData["Title"] = "Super Admins Management";
            ViewData["UserType"] = "Super Admins";
            ViewData["Search"] = search;
            return View("Index", superAdmins);
        }

        public async Task<IActionResult> Customers(int page = 1, string search = null)
        {
            var customers = await _userService.GetCustomersAsync(page, 10, search);
            ViewData["Title"] = "Customers Management";
            ViewData["UserType"] = "Customers";
            ViewData["Search"] = search;
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
                    await _userService.BlockUserAsync(model.UserId, model.BlockReason);
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
            await _userService.UnblockUserAsync(userId);
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
                    await _userService.ChangeUserRoleAsync(model);

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
