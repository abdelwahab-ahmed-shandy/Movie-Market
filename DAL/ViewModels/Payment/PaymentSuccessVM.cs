using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Payment
{
    public class PaymentSuccessVM
    {
        public DAL.Models.Order Order { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string PaymentMethod { get; set; } = "Credit Card";
        public DateTime? ShowTime { get; set; } // Added for cinema show time
    }
}
