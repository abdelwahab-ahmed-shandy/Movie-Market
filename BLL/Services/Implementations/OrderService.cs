using Microsoft.EntityFrameworkCore.Query;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<DAL.Models.Order> _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;

        public OrderService(
            IGenericRepository<DAL.Models.Order> orderRepository,
            IGenericRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }


        public async Task<PaginatedList<DAL.Models.Order>> GetAllOrdersAsync(int pageIndex = 1,string sortOrder = "date_desc",string searchString = "")
        {
            var baseQuery = _orderRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                baseQuery = baseQuery.Where(o =>
                    o.Id.ToString().Contains(searchString) ||
                    o.ApplicationUser.FirstName.Contains(searchString) ||
                    o.ApplicationUser.LastName.Contains(searchString) ||
                    o.OrderTotal.ToString().Contains(searchString) ||
                    o.Status.ToString().Contains(searchString));
            }

            var sortedQuery = ApplySorting(baseQuery, sortOrder);

            var query = sortedQuery
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie);

            return await PaginatedList<DAL.Models.Order>.CreateAsync(query, pageIndex, 10);
        }

        public async Task<PaginatedList<DAL.Models.Order>> GetShippedOrdersAsync(int pageIndex = 1, string sortOrder = "date_desc")
        {
            var baseQuery = _orderRepository.Get(o => o.OrderShipedStatus);
            var sortedQuery = ApplySorting(baseQuery, sortOrder);

            var query = sortedQuery
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie);

            return await PaginatedList<DAL.Models.Order>.CreateAsync(query, pageIndex, 10);
        }

        public async Task<PaginatedList<DAL.Models.Order>> GetCancelRequestsAsync(int pageIndex = 1, string sortOrder = "date_desc")
        {
            var baseQuery = _orderRepository.Get(o => o.Status == OrderStatus.Canceled);
            var sortedQuery = ApplySorting(baseQuery, sortOrder);

            var query = sortedQuery
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie);

            return await PaginatedList<DAL.Models.Order>.CreateAsync(query, pageIndex, 10);
        }

        private IQueryable<DAL.Models.Order> ApplySorting(IQueryable<DAL.Models.Order> query, string sortOrder)
        {
            return sortOrder switch
            {
                "Id" => query.OrderBy(o => o.Id),
                "Id_desc" => query.OrderByDescending(o => o.Id),
                "Customer" => query.OrderBy(o => o.ApplicationUser.LastName)
                                 .ThenBy(o => o.ApplicationUser.FirstName),
                "Customer_desc" => query.OrderByDescending(o => o.ApplicationUser.LastName)
                                      .ThenByDescending(o => o.ApplicationUser.FirstName),
                "date" => query.OrderBy(o => o.OrderDate),
                "date_desc" => query.OrderByDescending(o => o.OrderDate),
                "total" => query.OrderBy(o => o.OrderTotal),
                "total_desc" => query.OrderByDescending(o => o.OrderTotal),
                "Status" => query.OrderBy(o => o.Status),
                "Status_desc" => query.OrderByDescending(o => o.Status),
                _ => query.OrderByDescending(o => o.OrderDate)
            };
        }



        public async Task<DAL.Models.Order> GetOrderDetailsAsync(Guid orderId)
        {
            return await _orderRepository.Get(o => o.Id == orderId)
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _orderRepository.Update(order);
            return true;
        }

        public async Task<bool> UpdateShippingInfoAsync(Guid orderId, string trackingNumber, string carrier)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.TrackingNumber = trackingNumber;
            order.Carrier = carrier;
            order.OrderShipedStatus = true;
            order.Status = OrderStatus.Shipped;
            await _orderRepository.Update(order);
            return true;
        }

        public async Task<int> GetPendingCancelRequestsCountAsync()
        {
            return await _orderRepository.Get(o => o.Status == OrderStatus.Canceled).CountAsync();
        }
    
    
    }

}

