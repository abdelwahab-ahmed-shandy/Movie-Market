using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<DAL.Models.Order>> GetAllOrdersAsync();
        Task<IEnumerable<DAL.Models.Order>> GetShippedOrdersAsync();
        Task<IEnumerable<DAL.Models.Order>> GetCancelRequestsAsync();
        Task<DAL.Models.Order> GetOrderDetailsAsync(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<bool> UpdateShippingInfoAsync(Guid orderId, string trackingNumber, string carrier);
        Task<int> GetPendingCancelRequestsCountAsync();
    }
}
