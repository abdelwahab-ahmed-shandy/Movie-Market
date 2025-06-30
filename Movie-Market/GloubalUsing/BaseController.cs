
namespace Movie_Market.GloubalUsing
{
    public abstract class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        protected BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                var user = _userManager.FindByIdAsync(userId).Result;

                ViewBag.ProfileImagePath = string.IsNullOrEmpty(user?.ProfileImage)
                    ? "Assets/identity/user.png"
                    : user.ProfileImage;
            }
        }

        protected IActionResult NotFound(string? message = null)
        {
            Response.StatusCode = 404;
            ViewBag.Message = message ?? "The page you're looking for doesn't exist.";
            return View("~/Views/Shared/NotFound.cshtml");
        }

        protected IActionResult AccessDenied(string returnUrl = null)
        {
            Response.StatusCode = 403;
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

        protected IActionResult ServerError(string? message = null)
        {
            Response.StatusCode = 500;
            ViewBag.Message = message ?? "An unexpected error occurred.";
            return View("~/Views/Shared/GenericError.cshtml");
        }

        protected IActionResult Unauthorized(string? message = null)
        {
            Response.StatusCode = 401;
            ViewBag.Message = message ?? "You need to authenticate to access this resource.";
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        protected IActionResult Maintenance()
        {
            Response.StatusCode = 503;
            return View("~/Views/Shared/Maintenance.cshtml");
        }

        protected IActionResult ComingSoon()
        {
            Response.StatusCode = 202; 
            return View("~/Views/Shared/ComingSoon.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error(int statusCode)
        {
            var model = new ErrorViewModel
            {
                StatusCode = statusCode,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                OriginalPath = HttpContext.Items["originalPath"]?.ToString(),
                ErrorMessage = statusCode == 500 ? "An internal server error occurred." : null
            };


            return View("~/Views/Shared/Error.cshtml", model);
        }

    }
}
