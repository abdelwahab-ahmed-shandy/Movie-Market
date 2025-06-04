
using DAL.Enums;
using Microsoft.AspNetCore.Identity;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService,
                                    UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        #region Confirm Email

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.AccountStateType = AccountStateType.Active;
                user.IsActive = true;
                await _userManager.UpdateAsync(user);

                await _accountService.RecordAuditLogAsync(
                    "Email Confirmed",
                    "User confirmed their email address",
                    user.Id);

                await _signInManager.SignInAsync(user, isPersistent: false);

                return View(nameof(ConfirmEmail));
            }

            ViewBag.ErrorMessage = "Error confirming your email.";
            return View("Error");
        }
        #endregion


        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.RegisterUserAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User registration successful.");
                    return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                }

                AddErrorsToModelState(result);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterConfirmation(string email)
        {
            var user = await _accountService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Email = email;
            return View();
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion




    }
}
