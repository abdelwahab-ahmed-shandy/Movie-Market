using DAL.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;
using System.Security.Claims;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IGenericRepository<Order> _orderRepository;

        public OrderController(IGenericRepository<Order> orderRepository,
                             UserManager<ApplicationUser> userManager)
                             : base(userManager)
        {
            _orderRepository = orderRepository;
        }

        #region Views
        public async Task<IActionResult> CustomerOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var orders = await _orderRepository.Get(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                        .ThenInclude(m => m.CinemaMovies)
                            .ThenInclude(cm => cm.Cinema)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Add cinema information to each order item
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    item.Cinema = item.Movie.CinemaMovies
                        .FirstOrDefault(cm => cm.MovieId == item.MovieId)?.Cinema;
                }
            }

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var order = await _orderRepository.Get(o => o.Id == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                        .ThenInclude(m => m.CinemaMovies)
                            .ThenInclude(cm => cm.Cinema)
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            // Add cinema information to each order item
            foreach (var item in order.OrderItems)
            {
                item.Cinema = item.Movie.CinemaMovies
                    .FirstOrDefault(cm => cm.MovieId == item.MovieId)?.Cinema;
            }

            return View(order);
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> GetRecentOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var orders = await _orderRepository.Get(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                .OrderByDescending(o => o.OrderDate)
                .Take(5) // Show last 5 orders
                .ToListAsync();

            return PartialView("_RecentOrdersPartial", orders);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Get the original order
                var originalOrder = await _orderRepository.Get(o => o.Id == id && o.ApplicationUserId == userId)
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync();

                if (originalOrder == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                // Create a new order based on the original
                var newOrder = new Order
                {
                    ApplicationUserId = userId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    OrderTotal = originalOrder.OrderTotal,
                    OrderItems = originalOrder.OrderItems.Select(oi => new OrderItem
                    {
                        MovieId = oi.MovieId,
                        Price = oi.Price,
                        Count = oi.Count
                    }).ToList()
                };

                // Save the new order
                await _orderRepository.Add(newOrder);

                return Json(new
                {
                    success = true,
                    newOrderId = newOrder.Id,
                    message = "Order has been recreated successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEditForm(Guid id)
        {
            var order = await _orderRepository.Get(o => o.Id == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return Content("Order not found");
            }

            return PartialView("_EditOrderPartial", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderUpdateModel model)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var order = await _orderRepository.Get(o => o.Id == id && o.ApplicationUserId == userId)
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                // Update order items
                foreach (var item in model.OrderItems)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi => oi.Id == Guid.Parse(item.Id));
                    if (orderItem != null)
                    {
                        orderItem.Count = item.Count;
                    }
                }

                // Recalculate total
                order.OrderTotal = order.OrderItems.Sum(oi => oi.Count * oi.Price);
                order.UpdatedDateUtc = DateTime.UtcNow;

                await _orderRepository.Update(order);

                return Json(new
                {
                    success = true,
                    message = "Order updated successfully",
                    newTotal = order.OrderTotal.ToString("C")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Customer/Order/CancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found" });
                }

                if (order.Status == OrderStatus.Canceled)
                {
                    return Json(new { success = false, message = "Order is already cancelled" });
                }

                order.Status = OrderStatus.Canceled;
                order.UpdatedDateUtc = DateTime.Now;

                await _orderRepository.Update(order);

                return Json(new
                {
                    success = true,
                    message = "Order cancelled successfully",
                    status = order.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}