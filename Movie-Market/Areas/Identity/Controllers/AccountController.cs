using DAL.Enums;
using DAL.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;
using System.Security.Claims;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        #region Register
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM VM)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(VM);
                if (result.Succeeded)
                {
                    TempData["notification"] = "Registration successful. Please check your email to confirm your account.";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(VM);
        }

        #endregion


        #region Login
        [HttpGet("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM VM, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(VM);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl ?? "/");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { returnUrl });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(VM);
                }
            }

            return View(VM);
        }

        #endregion


        #region LogOut
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        #endregion


        #region Forgot Password
        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                await _authService.ForgotPasswordAsync(model);
                TempData["notification"] = "If your email exists in our system, you will receive a password reset link.";
                TempData["MessageType"] = "success";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }
        #endregion


        #region Forgot Password Confirmation
        [HttpGet("ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion


        #region Reset Password
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost("ResetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.ResetPasswordAsync(model);
            if (result.Succeeded)
            {
                TempData["notification"] = "Your password has been reset.";
                TempData["MessageType"] = "success";
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion


        #region Reset Password Confirmation
        [HttpGet("ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _authService.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                TempData["notification"] = "Thank you for confirming your email.";
                TempData["MessageType"] = "success";
                return View();
            }

            TempData["notification"] = "Error confirming your email.";
            TempData["MessageType"] = "error";
            return View("Error");
        }

        #endregion



        //#region External Login

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });

        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        //    return Challenge(properties, provider);
        //}

        //[HttpGet]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //{
        //    returnUrl ??= Url.Content("~/");

        //    if (!string.IsNullOrEmpty(remoteError))
        //    {

        //        TempData["notification"] = $"Service provider error: {remoteError}";
        //        TempData["MessageType"] = "error";
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {

        //        TempData["notification"] = "Failed to retrieve login information from Google.";
        //        TempData["MessageType"] = "error";
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var signInResult = await _signInManager.ExternalLoginSignInAsync(
        //    info.LoginProvider,
        //    info.ProviderKey,
        //    isPersistent: false,
        //    bypassTwoFactor: true
        //    );

        //    // If the login process was successful
        //    if (signInResult.Succeeded)
        //    {
        //        TempData["notification"] = "Successfully logged in with Google.";
        //        TempData["MessageType"] = "success";
        //        return LocalRedirect(returnUrl);
        //    }

        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        //    if (email == null)
        //    {
        //        TempData["notification"] = "Email not retrieved from Google account.";
        //        TempData["MessageType"] = "error";
        //        return RedirectToAction(nameof(Login));
        //    }

        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user != null)
        //    {
        //        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        //        if (addLoginResult.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            TempData["notification"] = "Google account linked and login successful.";
        //            TempData["MessageType"] = "success";
        //            return LocalRedirect(returnUrl);
        //        }

        //        foreach (var error in addLoginResult.Errors)
        //            ModelState.AddModelError(string.Empty, error.Description);

        //        TempData["notification"] = "An error occurred while linking the Google account.";
        //        TempData["MessageType"] = "error";
        //        return RedirectToAction(nameof(Login));
        //    }

        //    user = new ApplicationUser
        //    {
        //        UserName = email,
        //        Email = email,
        //        EmailConfirmed = true,
        //        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
        //        LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
        //        Address = info.Principal.FindFirstValue(ClaimTypes.StreetAddress),
        //        IsBlocked = false

        //    };

        //    var createResult = await _userManager.CreateAsync(user);
        //    if (createResult.Succeeded)
        //    {
        //        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        //        if (addLoginResult.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            TempData["notification"] = "Account created and successful sign-in via Google.";
        //            TempData["MessageType"] = "success";
        //            return LocalRedirect(returnUrl);
        //        }

        //        TempData["notification"] = "Account created, but Google account linking failed.";
        //        TempData["MessageType"] = "error";
        //    }
        //    else
        //    {
        //        TempData["notification"] = "Account creation failed.";
        //        TempData["MessageType"] = "error";
        //        foreach (var error in createResult.Errors)
        //            ModelState.AddModelError(string.Empty, error.Description);
        //    }

        //    return RedirectToAction(nameof(Login));
        //}

        //#endregion


        //#region ConfirmEmail (Beginning of the email confirmation section )
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return NotFound("Invalid email confirmation request.");
        //    }

        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{userId}'.");
        //    }

        //    var result = await _userManager.ConfirmEmailAsync(user, code);

        //    if (result.Succeeded)
        //    {
        //        await _signInManager.SignInAsync(user, isPersistent: false);

        //        TempData["notification"] = "Your email has been successfully confirmed! You have been automatically logged in.";
        //        TempData["MessageType"] = "Success";

        //        return RedirectToAction("Profile", "Settings", new { area = "Identity" });
        //    }

        //    return View("Error");
        //}
        //#endregion 


    }
}