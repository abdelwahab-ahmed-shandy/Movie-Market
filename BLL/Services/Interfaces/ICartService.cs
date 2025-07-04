
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
