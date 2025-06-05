using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Cart
{
    public class CartDropdownVM
    {
        public List<CartItem> Items { get; set; }
        public decimal Total { get; set; }
    }
}
