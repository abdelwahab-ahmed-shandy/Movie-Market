using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Order
{
    public class OrderUpdateModel
    {
        public List<OrderItemUpdateModel> OrderItems { get; set; }
    }

    public class OrderItemUpdateModel
    {
        public string Id { get; set; }
        public int Count { get; set; }
        public string MovieId { get; set; }
    }
}
