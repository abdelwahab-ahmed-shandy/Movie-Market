using MovieMarket.Repositories.IRepositories;

namespace MovieMarket.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(MovieMarketDbContext movieMartDbContext) : base(movieMartDbContext)
        {
        }
    }
}
