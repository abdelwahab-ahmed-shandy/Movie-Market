using DAL.Enums;
using DAL.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Movie_Market.GloubalUsing;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public AccountController(ILogger<AccountController> logger , IEmailService emailService ,
                                    IWebHostEnvironment webHostEnvironment , UserManager<ApplicationUser> userManager ,
                                        SignInManager<ApplicationUser> signInManager , RoleManager<IdentityRole<Guid>> roleManager) : base(userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Private Methods

        private bool IsOnLoginPage =>
                ControllerContext.ActionDescriptor.ActionName == "Login" &&
                    ControllerContext.ActionDescriptor.ControllerName == "Account";

        private bool IsOnRegisterPage =>
                ControllerContext.ActionDescriptor.ActionName == "Register" &&
                    ControllerContext.ActionDescriptor.ControllerName == "Account";

        #endregion


        #region Register

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await _roleManager.CreateAsync(role: new IdentityRole<Guid>("SuperAdmin"));
                await _roleManager.CreateAsync(role: new IdentityRole<Guid>("Admin"));
                await _roleManager.CreateAsync(role: new IdentityRole<Guid>("Customer"));
            }

            if (User.Identity?.IsAuthenticated == true)
            {
                TempData["notification"] = "You are already registered";
                TempData["MessageType"] = "Information";

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            // To ensure that the login button appears on the registration page
            ViewData["ShowLoginButton"] = true;
            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = registerVM.FirstName,
                    LastName = registerVM.LastName,
                    Address = registerVM.Address,
                    UserName = $"{registerVM.FirstName}.{registerVM.LastName}.{Guid.NewGuid().ToString().Substring(0, 4)}",
                    Email = registerVM.Email,
                    RegistrationDate = DateTime.UtcNow,
                    BirthDay = registerVM.BirthDay,
                    AccountStateType = AccountStateType.Active,
                    IsActive = false,
                    EmailConfirmed = false,
                    
                };

                var newUser = await _userManager.CreateAsync(applicationUser, registerVM.Password);

                if (newUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, "Customer");

                    var userId = applicationUser.Id;
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var returnUrl = Url.Content("~/");

                    var callbackUrl = Url.Action( "ConfirmEmail", "Account",
                    new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme );

                    await _emailService.SendEmailAsync(registerVM.Email, "Confirm your email",
                    $"MovieMart : Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                    TempData["notification"] = "Registration successful! Please check your email to verify your account.";
                    TempData["MessageType"] = "Success";
                    await Task.Delay(2000);

                    await _userManager.AddToRoleAsync(applicationUser, "Customer");

                    if (applicationUser.EmailConfirmed)
                    {
                        await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }

                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    foreach (var error in newUser.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }

            return View(registerVM);
        }

        #endregion


        #region Log-in

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["notification"] = "You are already logged in";
                TempData["MessageType"] = "Information";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            // the registration button appears on the Login page
            ViewData["ShowRegisterButton"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginVM);
                }

                if (user.IsBlocked)
                {
                    ModelState.AddModelError(string.Empty, "Account blocked. Contact support.");
                    return View(loginVM);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName!, loginVM.Password, loginVM.RememberMe,lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    else
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is temporarily locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(loginVM);
        }
        #endregion


        #region Log-Out

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        #endregion



        #region Forget Password

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);


            if (user != null && user.EmailConfirmed)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, token = token }, protocol: Request.Scheme);

                await _emailService.SendEmailAsync(
                    model.Email,
                    "Reset your MovieMart password",
                    $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");
            }

            TempData["notification"] = "If your email is registered, you'll receive a password reset link.";
            TempData["MessageType"] = "info";
            
            return RedirectToAction(nameof(ForgetPasswordConfirmation));

        }

        #endregion


        #region Forget Password Confirmation

        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        #endregion


        #region Reset Password

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            var forgetPassword = new ForgetPasswordVM
            {
                Email = email,
                ResetToken = token
            };

            return View(forgetPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ForgetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["notification"] = "Password has been reset successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.NewPassword);
            if (result.Succeeded)
            {
                user.PasswordChangedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                TempData["notification"] = "Password has been reset successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        #endregion


        #region Reset Password Confirmation

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion


        #region External Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {

                TempData["notification"] = $"Service provider error: {remoteError}";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {

                TempData["notification"] = "Failed to retrieve login information from Google.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync( info.LoginProvider, info.ProviderKey,
                                                                                isPersistent: false,
                                                                                    bypassTwoFactor: true);

            // If the login process was successful
            if (signInResult.Succeeded)
            {
                TempData["notification"] = "Successfully logged in with Google.";
                TempData["MessageType"] = "success";
                return LocalRedirect(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                TempData["notification"] = "Email not retrieved from Google account.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["notification"] = "Google account linked and login successful.";
                    TempData["MessageType"] = "success";
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in addLoginResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                TempData["notification"] = "An error occurred while linking the Google account.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Login));
            }

            user = new ApplicationUser
            {
                UserName = $"{email.Split('@')[0]}_{Guid.NewGuid().ToString().Substring(0, 6)}",
                Email = email,
                EmailConfirmed = true,
                RegistrationDate = DateTime.UtcNow,
                AccountStateType = AccountStateType.Active,
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User",
                Address = info.Principal.FindFirstValue(ClaimTypes.StreetAddress),
                IsBlocked = false,
                IsActive = true
            };


            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new IdentityRole<Guid>("Customer"));

                await _userManager.AddToRoleAsync(user, "Customer");

                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["notification"] = "Account created and successful sign-in via Google.";
                    TempData["MessageType"] = "success";
                    return LocalRedirect(returnUrl);
                }

                TempData["notification"] = "Account created, but Google account linking failed.";
                TempData["MessageType"] = "error";
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Login));
        }

        #endregion


        #region ConfirmEmail (Beginning of the email confirmation section )

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return NotFound("Invalid email confirmation request.");
            }

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                user.AccountStateType = AccountStateType.Active;
                user.EmailConfirmed = true;
                user.IsActive = true;
                user.IsBlocked = false;

                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["notification"] = "Your email has been successfully confirmed! You have been automatically logged in.";
                TempData["MessageType"] = "Success";

                return RedirectToAction("Index", "Profile", new { area = "Identity" });
            }

            return View("Error");
        }
        #endregion


        #region Error Pages inhert in BaseController :

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            ViewBag.ReturnUrl = returnUrl;
            _logger.LogWarning($"Access denied for user {User.Identity?.Name} attempting to access {returnUrl}");
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult NotFound() => base.NotFound();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Unauthorized() => base.Unauthorized();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ServerError() => base.ServerError();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Maintenance() => base.Maintenance();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ComingSoon() => base.ComingSoon();

        #endregion


    }
}

