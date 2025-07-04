using BLL.Services.Implementations;
using BLL.Utilities;
using DAL.ViewModels.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;
using Stripe;
using System.Security.Claims;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public PaymentController(UserManager<ApplicationUser> userManager , IPaymentService paymentService,
                                    IGenericRepository<Cart> cartRepository, IGenericRepository<Order> orderRepository) : base(userManager)
        {
            _paymentService = paymentService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
        }


        public async Task<IActionResult> Checkout()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cartItems = await _cartRepository.Get(c =>
                c.ApplicationUserId == userId &&
                c.CartStatus == CartStatus.Active)
                .Include(c => c.Movie)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                var session = await _paymentService.CreateCheckoutSessionAsync(userId, cartItems);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error processing your payment";
                return RedirectToAction("Index", "Cart");
            }
        }

        public async Task<IActionResult> Success(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                TempData["Error"] = "Invalid session ID";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // First try to get the order with all necessary includes
                var order = await _orderRepository.Get(o => o.SessionId == sessionId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Movie)
                            .ThenInclude(m => m.CinemaMovies)
                    .Include(o => o.ApplicationUser)
                    .FirstOrDefaultAsync();

                // If order not found or not completed, process it
                if (order == null || !order.PaymentStatus)
                {
                    order = await _paymentService.HandlePaymentSuccess(sessionId);

                    // If still not processed, show processing page
                    if (order == null || !order.PaymentStatus)
                    {
                        TempData["Info"] = "Your payment is being processed. Please check your email or refresh this page in a few moments.";
                        return View("Processing", sessionId);
                    }

                    // Refresh the order data after processing
                    order = await _orderRepository.Get(o => o.SessionId == sessionId)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Movie)
                                .ThenInclude(m => m.CinemaMovies)
                        .Include(o => o.ApplicationUser)
                        .FirstOrDefaultAsync();
                }

                // Double-check and clear any remaining active cart items
                var activeCartItems = await _cartRepository.Get(c =>
                    c.ApplicationUserId == userId &&
                    c.CartStatus == CartStatus.Active)
                    .ToListAsync();

                if (activeCartItems.Any())
                {
                    foreach (var item in activeCartItems)
                    {
                        item.CartStatus = CartStatus.Purchased;
                        await _cartRepository.Update(item);
                    }

                    // Consider adding a small delay if you're seeing race conditions
                    await Task.Delay(500);
                }

                if (order == null)
                {
                    TempData["Error"] = "Order not found after payment processing";
                    return RedirectToAction("Index", "Home");
                }

                var viewModel = new PaymentSuccessVM
                {
                    Order = order,
                    CustomerName = $"{order.ApplicationUser.FirstName} {order.ApplicationUser.LastName}",
                    CustomerEmail = order.ApplicationUser.Email,
                    ShowTime = order.OrderItems.FirstOrDefault()?.Movie.CinemaMovies.FirstOrDefault()?.ShowTime
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception here
                TempData["Error"] = $"Error processing your order. Please contact support with your session ID: {sessionId}";
                return RedirectToAction("Index", "Home");
            }
        }
       
        public async Task<IActionResult> Cancel(string sessionId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _paymentService.HandlePaymentFailure(sessionId);

                var cartItems = await _cartRepository.Get(c =>
                    c.ApplicationUserId == userId &&
                    c.CartStatus == CartStatus.Active)
                    .Include(c => c.Movie)
                    .Include(c => c.Cinema)
                    .ToListAsync();


                var viewModel = new PaymentCancelVM
                {
                    SessionId = sessionId,
                    CartItemsCount = cartItems.Count,
                    SampleCartItems = cartItems.Take(3).Select(item => new CartItemPreview
                    {
                        MovieTitle = item.Movie.Title,
                        Price = (decimal)item.Movie.Price,
                        CinemaName = item.Cinema.Name
                    }).ToList(),
                    Message = cartItems.Any() ?
                        "Your payment was interrupted. Your cart items have been saved." :
                        "Your payment was interrupted. No items were in your cart."
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error processing your cancellation";
                return RedirectToAction("Index", "Cart");
            }
        }

    }
}
