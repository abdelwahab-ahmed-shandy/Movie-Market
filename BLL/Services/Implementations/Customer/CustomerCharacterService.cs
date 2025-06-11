using BLL.Services.Interfaces.Customer;
using DAL.ViewModels.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories.IRepositories;
using BLL.Services.Interfaces.Customer;


namespace BLL.Services.Implementations.Customer
{
    public class CustomerCharacterService : ICustomerCharacterService
    {
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CharacterTvSeries> _characterTvSeriesRepo;

        public CustomerCharacterService(
            IGenericRepository<Character> characterRepo,
            IGenericRepository<CharacterMovie> characterMovieRepo,
            IGenericRepository<CharacterTvSeries> characterTvSeriesRepo)
        {
            _characterRepo = characterRepo;
            _characterMovieRepo = characterMovieRepo;
            _characterTvSeriesRepo = characterTvSeriesRepo;
        }

        public async Task<List<CharacterIndexVM>> GetAllCharactersAsync()
        {
            var characters = await _characterRepo.GetAll()
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            return characters.Select(c => new CharacterIndexVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Img = c.ImgUrl
            }).ToList();
        }

        public async Task<CharacterIndexVM?> GetCharacterDetailsAsync(Guid id)
        {
            var character = await _characterRepo.GetByIdAsync(id);
            if (character == null || character.IsDeleted)
                return null;

            var characterMovies = await _characterMovieRepo.Get(cm => cm.CharacterId == id)
                .Include(cm => cm.Movie)
                .ToListAsync();

            var characterTvSeries = await _characterTvSeriesRepo.Get(ct => ct.CharacterId == id)
                .Include(ct => ct.TvSeries)
                .ToListAsync();

            return new CharacterIndexVM
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description,
                Img = character.ImgUrl,
                Movies = characterMovies.Select(cm => cm.Movie.Title).ToList(),
                TvSeries = characterTvSeries.Select(ct => ct.TvSeries.Title).ToList()
            };
        }

    }
}
