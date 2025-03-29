using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(MovieMarketDbContext movieMarketDbContext) : base(movieMarketDbContext)
        {

        }
    }
}
