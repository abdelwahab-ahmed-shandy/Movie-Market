using DAL.ViewModels.Cart;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICartService
    {
        void AddToCart(Movie movie, int quantity = 1);
        Task AddToCartAsync(Movie movie, int quantity = 1);
        void RemoveFromCart(Guid movieId);
        List<CartItem> GetCartItems();
        Task<List<CartItem>> GetCartItemsAsync();
        decimal CalculateTotal();
        string GetCurrentUserId();
    }
}
