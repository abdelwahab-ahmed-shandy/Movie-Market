using BLL.Services.Interfaces.Customer;
using BLL.Services.Interfaces.Customer;
using DAL.Repositories.IRepositories;
using DAL.ViewModels.Character;
using DAL.ViewModels.TvSeries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Services.Implementations.Customer
{
    public class CustomerCharacterService : ICustomerCharacterService
    {
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CharacterTvSeries> _characterTvSeriesRepo;
        private readonly ILogger<CustomerCharacterService> _logger;
        public CustomerCharacterService(ILogger<CustomerCharacterService> logger,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<CharacterMovie> characterMovieRepo,
            IGenericRepository<CharacterTvSeries> characterTvSeriesRepo)
        {
            _logger = logger;
            _characterRepo = characterRepo;
            _characterMovieRepo = characterMovieRepo;
            _characterTvSeriesRepo = characterTvSeriesRepo;
        }

        public async Task<List<CharacterIndexVM>> GetAllCharactersAsync()
        {
            var characters = await _characterRepo.GetAll()
                .Where(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
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
            try
            {
                var character = await _characterRepo.GetByIdAsync(id);
                if (character == null || character.IsDeleted)
                    return null;

                var characterMovies = await _characterMovieRepo
                    .Get(cm => cm.CharacterId == id && cm.CurrentState == CurrentState.Active)
                    .Include(cm => cm.Movie)
                    .ThenInclude(m => m.Category) // إذا كنت بحاجة إلى معلومات التصنيف
                    .ToListAsync();

                var characterTvSeries = await _characterTvSeriesRepo
                    .Get(ct => ct.CharacterId == id && ct.CurrentState == CurrentState.Active)
                    .Include(ct => ct.TvSeries)
                    .ToListAsync();

                return new CharacterIndexVM
                {
                    Id = character.Id,
                    Name = character.Name,
                    Description = character.Description,
                    Img = character.ImgUrl,
                    Movies = characterMovies.Select(cm => new MovieIndexVM
                    {
                        Id = cm.Movie.Id,
                        Title = cm.Movie.Title,
                        ImgUrl = cm.Movie.ImgUrl,
                        Price = cm.Movie.Price,
                        Rating = cm.Movie.Rating,
                        CategoryName = cm.Movie.Category?.Name ?? "Unknown",
                        ReleaseYear = cm.Movie.ReleaseYear,
                        ShortDescription = cm.Movie.Description?.Length > 100 ?
                            cm.Movie.Description.Substring(0, 100) + "..." :
                            cm.Movie.Description
                    }).ToList(),
                    TvSeries = characterTvSeries.Select(ct => new TvSeriesVM
                    {
                        Id = ct.TvSeries.Id,
                        Title = ct.TvSeries.Title,
                        Description = ct.TvSeries.Description,
                        Author = ct.TvSeries.Author,
                        ImgUrl = ct.TvSeries.ImgUrl,
                        ReleaseYear = ct.TvSeries.ReleaseYear,
                        Rating = ct.TvSeries.Rating
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving character details for ID: {id}");
                throw;
            }
        }


    }
}
