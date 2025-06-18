using DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;
using System.Security.Claims;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        public AccountController(IAccountService accountService,
                                    UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                            ILogger<AccountController> logger,
                                                IEmailService emailService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }



        #region External Login

        // This action is called when the user clicks the "Sign in with Google or another external provider" button
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Generate a unique return URL with anti-forgery token
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }


        // This action is called automatically after the user returns from an external login provider (e.g., Google)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                _logger.LogError($"External provider error: {remoteError}");
                TempData["Notification"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogWarning("Error loading external login information.");
                TempData["Notification"] = "Error loading external login information.";
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                    info.Principal.Identity.Name, info.LoginProvider);

                TempData["Notification"] = $"Successfully logged in with {info.LoginProvider}.";
                return LocalRedirect(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return RedirectToAction("ExternalLoginConfirmation", new { ReturnUrl = returnUrl });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "",
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                    Address = info.Principal.FindFirstValue(ClaimTypes.StreetAddress) ?? "",
                    ProfileImage = GetProfilePictureUrl(info),
                    IsBlocked = false
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user: {Errors}",
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));

                    TempData["Notification"] = "Failed to create account. Please try again.";
                    return RedirectToAction("Login");
                }
            }

            var existingLogin = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (existingLogin == null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (!addLoginResult.Succeeded)
                {
                    TempData["Notification"] = "Something went wrong while adding external login.";
                    return RedirectToAction("Login");
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User logged in using {Name} provider.", info.LoginProvider);

            TempData["Notification"] = $"Successfully logged in with {info.LoginProvider}.";
            return LocalRedirect(returnUrl);
        }

        private string GetProfilePictureUrl(ExternalLoginInfo info)
        {
            if (info.LoginProvider == "Google")
            {
                return info.Principal.FindFirstValue("picture");
            }
            else if (info.LoginProvider == "Facebook")
            {
                var id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                return $"https://graph.facebook.com/{id}/picture?type=large";
            }
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(string returnUrl = null, string loginProvider = null)
        {
            return View(nameof(RegisterConfirmation));
        }
        #endregion



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