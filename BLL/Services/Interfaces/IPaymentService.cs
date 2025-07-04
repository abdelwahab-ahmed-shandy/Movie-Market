using Stripe.BillingPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(Guid userId, List<Cart> cartItems);
        Task<Order> HandlePaymentSuccess(string sessionId);
        Task HandlePaymentFailure(string sessionId);
        Task<Order> ProcessOrder(Guid userId, string sessionId, string paymentIntentId);
    }
}
