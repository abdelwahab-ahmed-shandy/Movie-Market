using MovieMarket.Models;
using MovieMarket.Repositories.IRepositories;

namespace MovieMarket.Repositories
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        public CinemaRepository(MovieMarketDbContext movieMarketDbContext) : base(movieMarketDbContext)
        {

        }
    }
}
