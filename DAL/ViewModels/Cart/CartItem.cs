using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cart
{
    public class CartItem
    {
        public Guid MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? ImgUrl { get; set; }
        public int Quantity { get; set; } = 1;

        public double Total => Quantity * Price;
    }

}
