using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IOrderService
    {
        #region Views 
        Task<PaginatedList<DAL.Models.Order>> GetAllOrdersAsync(int pageIndex = 1, string sortOrder = "date_desc", string searchString = "");
        Task<PaginatedList<DAL.Models.Order>> GetShippedOrdersAsync(int pageIndex, string sortOrder);
        Task<PaginatedList<DAL.Models.Order>> GetCancelRequestsAsync(int pageIndex, string sortOrder);
        #endregion

        Task<DAL.Models.Order> GetOrderDetailsAsync(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<bool> UpdateShippingInfoAsync(Guid orderId, string trackingNumber, string carrier);
        Task<int> GetPendingCancelRequestsCountAsync();
    }
}
