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
        Task AddToCartAsync(Movie movie, int quantity = 1);
        Task AddToCartAsync(Guid movieId, Guid cinemaId, int quantity = 1);
        Task<bool> RemoveFromCartAsync(Guid cartId);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<decimal> CalculateTotalAsync();
        Task<bool> ClearCartAsync();
    }
}
