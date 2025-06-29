using DAL.ViewModels.TvSeries;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CharacterService : ICharacterService
    {
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CharacterTvSeries> _characterTvSeriesRepo;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly ILogger<CharacterService> _logger;

        public CharacterService(IGenericRepository<Character> characterRepo, IGenericRepository<CharacterMovie> characterMovieRepo,
                                       IGenericRepository<CharacterTvSeries> characterTvSeriesRepo,
                                            IGenericRepository<Movie> movieRepo, IGenericRepository<TvSeries> tvSeriesRepo,
                                                IHttpContextAccessor httpContextAccessor, IFileService fileService,
                                                    ILogger<CharacterService> logger)
        {
            _characterRepo = characterRepo;
            _characterMovieRepo = characterMovieRepo;
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _characterTvSeriesRepo = characterTvSeriesRepo;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _logger = logger;
        }


        #region Admin Methods
        public async Task<CharacterAdminListVM> GetCharacterAllAsync(int page, int pageSize, string? query = null)
        {
            var charactersQuery = _characterRepo.GetAllWithDeleted()
                .Include(c => c.CharacterMovies)
                    .ThenInclude(cm => cm.Movie)
                .Include(c => c.CharacterTvSeries)
                    .ThenInclude(ct => ct.TvSeries)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                charactersQuery = charactersQuery.Where(c =>
                    c.Name.Contains(query) ||
                    c.Description != null && c.Description.Contains(query));
            }

            var paginatedList = await PaginatedList<Character>.CreateAsync(charactersQuery, page, pageSize);

            return new CharacterAdminListVM
            {
                Characteres = paginatedList.Select(c => new CharacterAdminVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Img = c.ImgUrl,
                    CreatedDateUtc = c.CreatedDateUtc,
                    CurrentState = c.CurrentState.Value,
                    MovieTitles = c.CharacterMovies.Select(cm => cm.Movie.Title).ToList(),
                    TvSeriesTitles = c.CharacterTvSeries.Select(ct => ct.TvSeries.Title).ToList()
                }).ToList(),
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                TotalCount = paginatedList.TotalCount,
                SearchTerm = query
            };
        }

        public async Task<CharacterAdminVM?> GetCharacterByIdAsync(Guid id)
        {
            var character = await _characterRepo.GetAllWithDeleted()
                .Include(c => c.CharacterMovies)
                    .ThenInclude(cm => cm.Movie)
                .Include(c => c.CharacterTvSeries)
                    .ThenInclude(ct => ct.TvSeries)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (character == null) return null;

            return new CharacterAdminVM
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description,
                Img = character.ImgUrl,
                CurrentState = character.CurrentState.Value,
                IsDeleted = character.IsDeleted,
                CreatedDateUtc = character.CreatedDateUtc,
                MovieTitles = character.CharacterMovies.Select(cm => cm.Movie.Title).ToList(),
                TvSeriesTitles = character.CharacterTvSeries.Select(ct => ct.TvSeries.Title).ToList()
            };
        }

        public async Task<bool> CreateCharacterAsync(CreateCharacterVM model)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            try
            {

                string imagePath = null;
                if (model.Img != null && model.Img.Length > 0)
                {
                    imagePath = await _fileService.SaveFileAsync(model.Img, "uploads/characters");
                }

                var character = new Character
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImgUrl = imagePath,
                    CurrentState = model.CurrentState,
                    CreatedBy = userName,
                    CreatedDateUtc = DateTime.UtcNow
                };

                await _characterRepo.Add(character);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCharacterAsync(EditCharacterVM model)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            try
            {
                var character = await _characterRepo.GetByIdAsync(model.Id);
                if (character == null) return false;

                if (model.Img != null && model.Img.Length > 0)
                {
                    if (!string.IsNullOrEmpty(character.ImgUrl))
                    {
                        _fileService.DeleteFile(character.ImgUrl);
                    }

                    character.ImgUrl = await _fileService.SaveFileAsync(model.Img, "uploads/characters");
                }

                character.Name = model.Name;
                character.Description = model.Description;
                character.CurrentState = model.CurrentState;
                character.UpdatedBy = userName;
                character.UpdatedDateUtc = DateTime.UtcNow;

                await _characterRepo.Update(character);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SoftDeleteCharacterAsync(Guid id)
        {
            try
            {
                await _characterRepo.SoftDeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestoreCharacterAsync(Guid id)
        {
            try
            {
                await _characterRepo.RestoreAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCharacterAsync(Guid id)
        {
            try
            {
                var character = await _characterRepo.GetByIdAsync(id);
                if (character == null) return false;

                if (!string.IsNullOrWhiteSpace(character.ImgUrl))
                {
                    _fileService.DeleteFile(character.ImgUrl);
                }

                await _characterRepo.DeleteInDB(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting character: {ex.Message}");
                return false;
            }
        }

        #endregion


        #region Customer Methods

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
                    .ThenInclude(m => m.Category) 
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
                    Movies = characterMovies.Select(cm => new MovieCharacterIndexVM
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
                    TvSeries = characterTvSeries.Select(ct => new TvSeriesCharacterVM
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
        
        #endregion


    }

}
