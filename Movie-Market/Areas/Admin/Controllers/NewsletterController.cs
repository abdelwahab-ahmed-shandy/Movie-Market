﻿using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class NewsletterController : BaseController
    {
        private readonly INewsletterService _newsletterService;
        private readonly ILogger<NewsletterController> _logger;

        public NewsletterController(INewsletterService newsletterService, ILogger<NewsletterController> logger, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _newsletterService = newsletterService;
            _logger = logger;
        }


        #region View Index 
        public async Task<IActionResult> Index(string query, int page = 1, int pageSize = 5)
        {
            var model = await _newsletterService.GetSubscribersAsync(query, page, pageSize);
            ViewBag.Query = query;
            return View(model);
        }

        #endregion


        #region Send Email Subscribers :
        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                TempData["notification"] = "Subject and body are required.";
                TempData["MessageType"] = "error";
                return View();
            }

            var (success, message) = await _newsletterService.SendNewsletterAsync(subject, body);

            TempData["notification"] = message;
            TempData["MessageType"] = success ? "success" : "error";

            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region View Email History
        public async Task<IActionResult> EmailHistory()
        {
            var emails = await _newsletterService.GetEmailHistoryAsync();
            return View(emails);
        }
        #endregion


        #region Delete Subscriber
        [HttpPost]
        public async Task<IActionResult> DeleteSubscriber(Guid id)
        {
            try
            {
                await _newsletterService.DeleteSubscriberAsync(id);
                TempData["notification"] = "Subscriber deleted successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscriber");

                TempData["notification"] = "An error occurred while deleting the subscriber. Please try again.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

    }
}
