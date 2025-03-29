using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        public EpisodeRepository(MovieMarketDbContext movieMarketDbContext) : base(movieMarketDbContext)
        {

        }
    }
}
