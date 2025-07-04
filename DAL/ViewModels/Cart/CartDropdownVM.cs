using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cart
{
    public class CartDropdownVM
    {
        public List<CartItemVM> Items { get; set; } = new List<CartItemVM>();
        public double Total { get; set; }
        public int ItemCount { get; set; }
    }

    public class CartItemVM
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string CinemaName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
