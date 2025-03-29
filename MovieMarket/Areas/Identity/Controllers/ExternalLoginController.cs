using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MovieMarket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ExternalLoginController : Controller
    {
        #region Google Login

        /// <summary>
        /// Sends an authentication request to Google to initiate the login process.
        /// </summary>
        public IActionResult GoogleLogin()
        {
            // Specifies the URL to be redirected to after login.
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account");

            // Create authentication properties specifying the redirect URL after successful authentication.
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // Initiate authentication with Google.
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Receives the response after a successful or failed Google login.
        /// </summary>
        public async Task<IActionResult> GoogleResponse()
        {
            // Attempt to authenticate the user using cookies.
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // If authentication fails, the user is redirected to the login page.
            if (!authenticateResult.Succeeded)
                return RedirectToAction("Login");

            // Extract user data from claims (e.g., email, name, ID)
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            });

            // Return user data as JSON for display or processing
            return Json(claims);
        }

        #endregion
    }
}
