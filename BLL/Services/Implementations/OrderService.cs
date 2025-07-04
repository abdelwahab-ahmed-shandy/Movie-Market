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

        public async Task<IEnumerable<DAL.Models.Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAll()
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Movie)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DAL.Models.Order>> GetShippedOrdersAsync()
        {
            return await _orderRepository.Get(o => o.OrderShipedStatus)
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DAL.Models.Order>> GetCancelRequestsAsync()
        {
            return await _orderRepository.Get(o => o.Status == OrderStatus.Canceled)
                .Include(o => o.ApplicationUser)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
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

