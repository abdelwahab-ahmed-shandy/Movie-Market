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

        public async Task<IActionResult> CustomerOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var orders = await _orderRepository.Get(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

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
    }
}
