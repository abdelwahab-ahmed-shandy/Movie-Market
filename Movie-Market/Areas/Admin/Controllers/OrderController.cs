using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrderController : BaseController 
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> AllOrders(int pageNumber = 1, string sortOrder = "date_desc", string searchString = "")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            // Add all sort parameters
            ViewData["IdSortParm"] = sortOrder == "Id" ? "Id_desc" : "Id";
            ViewData["CustomerSortParm"] = sortOrder == "Customer" ? "Customer_desc" : "Customer";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["TotalSortParm"] = sortOrder == "total" ? "total_desc" : "total";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "Status_desc" : "Status";

            var orders = await _orderService.GetAllOrdersAsync(pageNumber, sortOrder, searchString);
            return View(orders);
        }

        public async Task<IActionResult> TrackOrders(int pageNumber = 1, string sortOrder = "date_desc")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["UserSortParm"] = sortOrder == "user" ? "user_desc" : "user";
            ViewData["TotalSortParm"] = sortOrder == "total" ? "total_desc" : "total";

            var orders = await _orderService.GetShippedOrdersAsync(pageNumber, sortOrder);
            return View(orders);
        }

        public async Task<IActionResult> CancelRequests(int pageNumber = 1, string sortOrder = "date_desc")
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["UserSortParm"] = sortOrder == "user" ? "user_desc" : "user";
            ViewData["TotalSortParm"] = sortOrder == "total" ? "total_desc" : "total";

            var orders = await _orderService.GetCancelRequestsAsync(pageNumber, sortOrder);
            return View(orders);
        }



        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(Guid orderId)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Canceled);
            if (!success)
            {

                TempData["notification"] = "Failed to approve cancellation";
                TempData["MessageType"] = "Error";

                return RedirectToAction(nameof(CancelRequests));
            }

            TempData["notification"] = "Cancellation approved successfully";
            TempData["MessageType"] = "Success";
            return RedirectToAction(nameof(CancelRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancel(Guid orderId)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.InProgress);
            if (!success)
            {

                TempData["notification"] = "Failed to reject cancellation";
                TempData["MessageType"] = "Error";
                return RedirectToAction(nameof(CancelRequests));
            }

            TempData["notification"] = "Cancellation rejected";
            TempData["MessageType"] = "Success";
            return RedirectToAction(nameof(CancelRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShipping(Guid orderId, string trackingNumber, string carrier)
        {
            var success = await _orderService.UpdateShippingInfoAsync(orderId, trackingNumber, carrier);
            if (!success)
            {

                TempData["notification"] = "Failed to update shipping information";
                TempData["MessageType"] = "Error";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            TempData["notification"] = "Shipping information updated successfully";
            TempData["MessageType"] = "Success";
            return RedirectToAction(nameof(TrackOrders));
        }

    }
}
