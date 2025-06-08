using Microsoft.AspNetCore.Mvc;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsletterController : Controller
    {
        private readonly INewsletterService _newsletterService;
        private readonly ILogger<NewsletterController> _logger;

        public NewsletterController(INewsletterService newsletterService, ILogger<NewsletterController> logger)
        {
            _newsletterService = newsletterService;
            _logger = logger;
        }


        #region View Index
        public async Task<IActionResult> Index(string query, int page = 1)
        {
            const int pageSize = 5;
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

    }
}
