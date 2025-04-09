using MovieMarket.DataAccess;
using MovieMarket.Models;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(MovieMarketDbContext movieMarketDbContext)
            : base(movieMarketDbContext)
        {

        }
    }
}
