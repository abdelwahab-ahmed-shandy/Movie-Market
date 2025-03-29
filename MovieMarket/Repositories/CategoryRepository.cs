using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(MovieMarketDbContext movieMarketDbContext)
            : base(movieMarketDbContext)
        {

        }
    }
}
