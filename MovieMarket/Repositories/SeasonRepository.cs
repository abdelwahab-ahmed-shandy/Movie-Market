using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class SeasonRepository : Repository<Season>, ISeasonRepository
    {
        public SeasonRepository(MovieMarketDbContext movieMarketDbContext) : base(movieMarketDbContext)
        {

        }
    }
}
