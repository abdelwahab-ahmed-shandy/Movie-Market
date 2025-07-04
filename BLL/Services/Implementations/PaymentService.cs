using Castle.Core.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(IGenericRepository<Order> orderRepository,IGenericRepository<OrderItem> orderItemRepository,
            IGenericRepository<Cart> cartRepository,
            IOptions<StripeSettings> stripeSettings)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _stripeSettings = stripeSettings.Value;

            // Set Stripe API key
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }


        public async Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(Guid userId, List<Cart> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cart cannot be empty");
            }

            try
            {
                // Create order first to reserve the items
                var order = await CreatePendingOrder(userId, cartItems);

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = cartItems.Select(item => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Movie.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Movie.Title,
                                Images = new List<string> { item.Movie.ImgUrl }
                            },
                        },
                        Quantity = item.Count,
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = $"{_stripeSettings.Domain}/Payment/Success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_stripeSettings.Domain}/Payment/Cancel",
                    ClientReferenceId = userId.ToString(),
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", userId.ToString() },
                        { "orderId", order.Id.ToString() }
                    }
                };

                var service = new Stripe.Checkout.SessionService();
                var session = await service.CreateAsync(options);

                // Update order with session ID
                order.SessionId = session.Id;
                await _orderRepository.Update(order);

                return session;
            }
            catch (StripeException ex)
            {
                throw new ApplicationException("Payment service error", ex);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<Order> CreatePendingOrder(Guid userId, List<Cart> cartItems)
        {
            var total = cartItems.Sum(item => item.Movie.Price * item.Count);

            var order = new Order
            {
                ApplicationUserId = userId,
                OrderTotal = total,
                Status = OrderStatus.Pending,
                PaymentStatus = false,
                OrderDate = DateTime.UtcNow
            };

            await _orderRepository.Add(order);

            // Create order items
            var orderItems = cartItems.Select(item => new OrderItem
            {
                OrderId = order.Id,
                MovieId = item.MovieId,
                Count = item.Count,
                Price = item.Movie.Price
            }).ToList();

            foreach (var item in orderItems)
            {
                await _orderItemRepository.Add(item);
            }

            return order;
        }

        public async Task<Order> HandlePaymentSuccess(string sessionId)
        {
            try
            {
                var service = new Stripe.Checkout.SessionService();
                var session = await service.GetAsync(sessionId);

                // Get the order from database
                var order = await _orderRepository.Get(o => o.SessionId == sessionId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                // Idempotency check - don't process if already completed
                if (order.PaymentStatus)
                {
                    return order;
                }

                // Update order status
                order.PaymentStatus = true;
                order.Status = OrderStatus.Completed;
                order.PaymentStripeId = session.PaymentIntentId;
                await _orderRepository.Update(order);

                // Get all active cart items for this user
                var cartItems = await _cartRepository.Get(c =>
                    c.ApplicationUserId == order.ApplicationUserId &&
                    c.CartStatus == CartStatus.Active)
                    .ToListAsync();

                // Mark all cart items as purchased
                foreach (var item in cartItems)
                {
                    item.CartStatus = CartStatus.Purchased;
                    await _cartRepository.Update(item);
                }

                return order;
            }
            catch (StripeException ex)
            {
                // Log Stripe-specific errors
                throw new ApplicationException("Stripe payment processing error", ex);
            }
            catch (Exception ex)
            {
                // Log general errors
                throw new ApplicationException("Error processing payment success", ex);
            }
        }

        public async Task HandlePaymentFailure(string sessionId)
        {
            try
            {
                var order = await _orderRepository.Get(o => o.SessionId == sessionId)
                    .FirstOrDefaultAsync();

                if (order != null && order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Canceled;
                    await _orderRepository.Update(order);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> ProcessOrder(Guid userId, string sessionId, string paymentIntentId)
        {

            try
            {
                // Get the order
                var order = await _orderRepository.Get(o =>
                    o.SessionId == sessionId &&
                    o.ApplicationUserId == userId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                // Get cart items
                var cartItems = await _cartRepository.Get(c =>
                    c.ApplicationUserId == userId &&
                    c.CartStatus == CartStatus.Active)
                    .ToListAsync();

                // Update order status
                order.PaymentStatus = true;
                order.Status = OrderStatus.InProgress;
                order.PaymentStripeId = paymentIntentId;
                await _orderRepository.Update(order);

                // Update cart items
                foreach (var cartItem in cartItems)
                {
                    cartItem.CartStatus = CartStatus.Purchased;
                    await _cartRepository.Update(cartItem);
                }

                //await _orderRepository.SaveChangesAsync();
                //await _cartRepository.SaveChangesAsync();


                return order;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    
    
    }

}
