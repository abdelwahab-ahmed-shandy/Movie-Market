using MovieMarket.DataAccess;
using MovieMart.Models;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Repositories
{
    public class CharacterRepository : Repository<Character>, ICharacterRepository
    {
        public CharacterRepository(MovieMarketDbContext movieMarketDbContext) : base(movieMarketDbContext)
        {

        }
    }
}
