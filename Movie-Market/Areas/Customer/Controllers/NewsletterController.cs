﻿using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NewsletterController : BaseController
    {
        private readonly ISubscriberService _subscriberService;
        private readonly ILogger<NewsletterController> _logger;
        public NewsletterController(ISubscriberService subscriberService
                                        , ILogger<NewsletterController> logger, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _logger = logger;
            _subscriberService = subscriberService;
        }

        #region Privte Method
        private IActionResult RedirectToReferer()
        {
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
        #endregion


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string email)
        {
            var (success, message) = await _subscriberService.SubscribeAsync(email);

            TempData["notification"] = message;
            TempData["MessageType"] = success ? "success" : "error";

            return RedirectToReferer();
        }


    }
}
