using DAL.Context;
using DAL.ViewModels.Cart;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly List<CartItem> _cartItems = new();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddToCart(Movie movie, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.MovieId == movie.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    MovieId = movie.Id,
                    Title = movie.Title,
                    Price = movie.Price,
                    ImgUrl = movie.ImgUrl,
                    Quantity = quantity
                });
            }
        }

        public async Task AddToCartAsync(Movie movie, int quantity = 1)
        {
            await Task.Run(() => AddToCart(movie, quantity));
        }

        public void RemoveFromCart(Guid movieId)
        {
            var item = _cartItems.FirstOrDefault(item => item.MovieId == movieId);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
        }

        public List<CartItem> GetCartItems()
        {
            return _cartItems;
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await Task.FromResult(_cartItems);
        }
        public decimal CalculateTotal()
        {
            return _cartItems.Sum(item => (decimal)(item.Price * item.Quantity));
        }
        public string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
