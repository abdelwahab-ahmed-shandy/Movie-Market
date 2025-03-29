using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MovieMarket.Models.ViewModels;
using MovieMarket.Services;
using System.Text.Encodings.Web;

namespace MovieMarket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._emailSender = emailSender;
        }

        #region Register

        //  Display the registration page when requested using HTTP GET
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            //  Check if roles do not exist, and create them if necessary
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(role: new IdentityRole("SuperAdmin")); // Create the "SuperAdmin" role
                await _roleManager.CreateAsync(role: new IdentityRole("Admin")); // Create the "Admin" role
                await _roleManager.CreateAsync(role: new IdentityRole("Customer")); // Create the "Customer" role
            }

            return View(); //  Display the registration page for the user
        }

        //  Process registration data when sent using HTTP POST
        [HttpPost]
        [ValidateAntiForgeryToken] //  Protection against CSRF attacks
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            //  Check if the form data is valid
            if (ModelState.IsValid)
            {
                //  Create a new user object based on the data entered in the form
                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = registerVM.FirstName,
                    LastName = registerVM.LastName,
                    Address = registerVM.Address,
                    UserName = registerVM.UserName,
                    Email = registerVM.Email
                };

                //  Create a user account in the system
                var newUser = await _userManager.CreateAsync(applicationUser, registerVM.Password);

                //  Check if the user was created successfully
                if (newUser.Succeeded)
                {
                    //  Generate Email Confirmation Token
                    var userId = applicationUser.Id;
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var returnUrl = Url.Content("~/"); // 🔙 Return URL after confirmation

                    //  Generate Email Confirmation Link
                    var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme
                    );

                    //  Send an email to the user with a confirmation link
                    await _emailSender.SendEmailAsync(registerVM.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //  Add a notification to the user informing them of the need to confirm their email
                    TempData["notifiction"] = "Your account has been created! Please check your email to confirm the account before logging in";
                    TempData["MessageType"] = "Success";

                    //  Automatically add the user to the "Customer" role
                    await _userManager.AddToRoleAsync(applicationUser, "Customer");

                    //  If the email address is already confirmed, login is immediate.
                    if (applicationUser.EmailConfirmed)
                    {
                        await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }

                    //  Direct the user to the login page if they haven't confirmed their email address yet.
                    return RedirectToAction("Login", "Account", new { area = "Identity" });
                }
                else
                {
                    //  If registration fails, errors are displayed to the user.
                    foreach (var error in newUser.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            //  Redisplay the registration page if there are input errors.
            return View(registerVM);
        }

        #endregion



        #region Login

        // Display the login page when requested via HTTP GET
        [HttpGet]

        public async Task<IActionResult> Login()
        {
            return View(); // Display the login interface
        }

        // Process login data when sent via HTTP POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protection against CSRF attacks
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            // Validate the entered data
            if (ModelState.IsValid)
            {
                // Find the user using email
                var user = await _userManager.FindByEmailAsync(loginVM.Email);

                if (user != null)
                {
                    // Validate the entered password
                    var checkPassByUser = await _userManager.CheckPasswordAsync(user, loginVM.Password);

                    if (checkPassByUser)
                    {
                        // User login successfully
                        await _signInManager.SignInAsync(user, loginVM.RememberMe);

                        // Redirect the user to the home page in the "Customer" area
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        // Add an error message if the password does not match
                        ModelState.AddModelError("Password", "Invalid password");
                        ModelState.AddModelError("Email", "Email not found");
                    }
                }
                else
                {
                    // Add an error message if the email address is not found
                    ModelState.AddModelError("Password", "Invalid password");
                    ModelState.AddModelError("Email", "Email not found");
                }
            }

            // Re-display the login page with the entered data in case of errors
            return View(loginVM);
        }

        #endregion


        #region LogOut

        // Log the user out
        public async Task<IActionResult> Logout()
        {
            // Perform the logout operation
            await _signInManager.SignOutAsync();

            // Redirect the user to the login page
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        #endregion


        #region AccDenied, NotFound, and IntSerErr

        // Display the "Access Denied" page when attempting to access an unauthorized resource
        public IActionResult AccessDenied() => View();
        // Display the "Not Found" page when attempting to access a non-existent page
        public IActionResult NotFound() => View();
        // Display the "Internal Server Error" page when an internal server error occurs
        public IActionResult InternalServerError() => View();

        #endregion



        /*
        Services:
        */
        #region ConfirmEmail (Beginning of the email confirmation section )

        // An asynchronous (Async) function used to confirm a user's email
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            // Checks that the entered values ​​are correct (userId and code must not be empty)
            if (userId == null || code == null)
            {
                return NotFound("Invalid email confirmation request."); // If the data is invalid, a 404 error is returned with a message explaining the problem
            }

            // Find the user in the database using their userId
            var user = await _userManager.FindByIdAsync(userId);

            // Checks if the user does not exist
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'."); // Return a 404 error if the user is not found
            }

            // Perform the email confirmation process using code
            var result = await _userManager.ConfirmEmailAsync(user, code);

            // Check if the confirmation process was successful
            if (result.Succeeded)
            {
                // Automatically log the user in after the email confirmation process is successful
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Store a notification in TempData to display to the user in the interface
                TempData["notification"] = "Your email has been successfully confirmed! You have been automatically logged in.";
                TempData["MessageType"] = "Success"; // Specify the message type as success

                // Redirect the user to the profile page within the "Identity" area
                return RedirectToAction("Profile", "Settings", new { area = "Identity" });
            }

            // If the confirmation process fails, an error page is displayed.
            return View("Error");
        }
        #endregion // End of the email confirmation section




    }
}
