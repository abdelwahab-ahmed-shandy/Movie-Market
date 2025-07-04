using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Payment
{
    public class PaymentCancelVM
    {
        public string Message { get; set; } = "You have cancelled the payment process. Your order has not been placed.";
        public int CartItemsCount { get; set; }
        public string SessionId { get; set; }
        public List<CartItemPreview> SampleCartItems { get; set; } = new List<CartItemPreview>();
    }

    public class CartItemPreview
    {
        public string MovieTitle { get; set; }
        public decimal Price { get; set; }
        public string CinemaName { get; set; }
    }
}
