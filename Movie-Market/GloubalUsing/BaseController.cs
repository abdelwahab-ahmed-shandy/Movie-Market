using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.GloubalUsing
{
    public class BaseController : Controller
    {
        protected IActionResult NotFound(string? message = null)
        {
            Response.StatusCode = 404;
            ViewBag.Message = message ?? "The page you're looking for doesn't exist.";
            return View("~/Views/Shared/NotFound.cshtml");
        }

        protected IActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View("~/Views/Shared/AccessDenied.cshtml");
        }

        protected IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View("~/Views/Shared/GenericError.cshtml");
        }

        protected IActionResult Unauthorized()
        {
            Response.StatusCode = 401;
            return View("~/Views/Shared/Unauthorized.cshtml");
        }

        protected IActionResult Maintenance()
        {
            Response.StatusCode = 503;
            return View("~/Views/Shared/Maintenance.cshtml");
        }

        protected IActionResult ComingSoon()
        {
            Response.StatusCode = 202; // HTTP 202 Accepted for Coming Soon
            return View("~/Views/Shared/ComingSoon.cshtml");
        }

    }
}
