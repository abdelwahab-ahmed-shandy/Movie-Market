using BLL.Services.Interfaces.Admin;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminCharacterService : IAdminCharacterService
    {
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<CharacterMovie> _characterMovieRepo;
        private readonly IGenericRepository<CharacterTvSeries> _characterTvSeriesRepo;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly ILogger<AdminCharacterService> _logger;

        public AdminCharacterService(IGenericRepository<Character> characterRepo, IGenericRepository<CharacterMovie> characterMovieRepo,
                                       IGenericRepository<CharacterTvSeries> characterTvSeriesRepo,
                                            IGenericRepository<Movie> movieRepo, IGenericRepository<TvSeries> tvSeriesRepo,
                                                IHttpContextAccessor httpContextAccessor, IFileService fileService,
                                                    ILogger<AdminCharacterService> logger)
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


        public async Task<CharacterAdminListVM> GetCharacterAll(int page, int pageSize, string? query = null)
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
                    (c.Description != null && c.Description.Contains(query)));
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


        public async Task<CharacterAdminVM?> GetCharacterById(Guid id)
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


        public async Task<bool> CreateCharacter(CreateCharacterVM model)
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


        public async Task<bool> UpdateCharacter(EditCharacterVM model)
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

        public async Task<bool> SoftDeleteCharacter(Guid id)
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


        public async Task<bool> RestoreCharacter(Guid id)
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


        public async Task<bool> DeleteCharacter(Guid id)
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

    }

}
