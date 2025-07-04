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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<Movie> _movieRepo;

        public CartService(
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<Cart> cartRepo,
            IGenericRepository<Movie> movieRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _cartRepo = cartRepo;
            _movieRepo = movieRepo;
        }

        private Guid? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null ? Guid.Parse(userId) : null;
        }

        public async Task AddToCartAsync(Movie movie, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == null) throw new UnauthorizedAccessException();

            var existingItem = await _cartRepo.GetOneAsync(c =>
                c.ApplicationUserId == userId &&
                c.MovieId == movie.Id &&
                c.CartStatus == CartStatus.Active);

            if (existingItem != null)
            {
                existingItem.Count += quantity;
                await _cartRepo.Update(existingItem);
            }
            else
            {
                var newCartItem = new Cart
                {
                    ApplicationUserId = userId.Value,
                    MovieId = movie.Id,
                    Count = quantity,
                    CartStatus = CartStatus.Active,
                    CreatedDateUtc = DateTime.UtcNow
                };
                await _cartRepo.Add(newCartItem);
            }
        }

        public async Task AddToCartAsync(Guid movieId, Guid cinemaId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            if (userId == null) throw new UnauthorizedAccessException();

            var existingItem = await _cartRepo.GetOneAsync(c =>
                c.ApplicationUserId == userId &&
                c.MovieId == movieId &&
                c.CinemaId == cinemaId &&
                c.CartStatus == CartStatus.Active);

            if (existingItem != null)
            {
                existingItem.Count += quantity;
                await _cartRepo.Update(existingItem);
            }
            else
            {
                var newCartItem = new Cart
                {
                    ApplicationUserId = userId.Value,
                    MovieId = movieId,
                    CinemaId = cinemaId,
                    Count = quantity,
                    CartStatus = CartStatus.Active,
                    CreatedDateUtc = DateTime.UtcNow
                };
                await _cartRepo.Add(newCartItem);
            }
        }

        public async Task<bool> RemoveFromCartAsync(Guid cartId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return false;

            var item = await _cartRepo.GetOneAsync(c =>
                c.Id == cartId &&
                c.ApplicationUserId == userId.Value &&
                c.CartStatus == CartStatus.Active);

            if (item != null)
            {
                await _cartRepo.DeleteInDB(item.Id);
                return true;
            }
            return false;
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return new List<CartItem>();

            var items = await _cartRepo.Get(c =>
                c.ApplicationUserId == userId.Value &&
                c.CartStatus == CartStatus.Active)
                .Include(c => c.Movie)
                .Include(c => c.Cinema)
                .ToListAsync();

            return items.Select(c => new CartItem
            {
                Id = c.Id,
                MovieId = c.MovieId,
                Title = c.Movie.Title,
                Price = c.Movie.Price,
                ImgUrl = c.Movie.ImgUrl,
                Quantity = c.Count,
                //Na = c.Cinema?.Name
            }).ToList();
        }

        public async Task<decimal> CalculateTotalAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return 0;

            var items = await _cartRepo.Get(c =>
                c.ApplicationUserId == userId.Value &&
                c.CartStatus == CartStatus.Active)
                .Include(c => c.Movie)
                .ToListAsync();

            return items.Sum(item => (decimal)(item.Movie.Price * item.Count));
        }

        public async Task<bool> ClearCartAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return false;

            var items = await _cartRepo.Get(c =>
                c.ApplicationUserId == userId.Value &&
                c.CartStatus == CartStatus.Active)
                .ToListAsync();

            foreach (var item in items)
            {
                await _cartRepo.DeleteInDB(item.Id);
            }

            return true;
        }
    
    }
}
