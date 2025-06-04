
namespace Movie_Market.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<ApplicationUser> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                   SignInManager<ApplicationUser> signInManager,
                                      EmailService emailService,
                                        IWebHostEnvironment webHostEnvironment,
                                          ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }



        #region Register

        [HttpGet]
        public async Task<IActionResult> Register()
        {

            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));


            if (User.Identity?.IsAuthenticated == true)
            {
                TempData["notification"] = "Your account has been created! Please check your email to confirm the account before logging in";
                TempData["MessageType"] = "Information";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return View(new RegisterVM());
        }

        #endregion

    }
}
