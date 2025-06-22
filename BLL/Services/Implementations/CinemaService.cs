using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CinemaService : ICinemaService
    {
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly ILogger<CinemaService> _logger;
        private readonly IGenericRepository<Movie> _movieRepo;

        public CinemaService(IGenericRepository<Cinema> cinemaRepo, IHttpContextAccessor httpContextAccessor,
                                    IFileService fileService , ILogger<CinemaService> logger, IGenericRepository<Movie> movieRepo)
        {
            _cinemaRepo = cinemaRepo;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _logger = logger;
            _movieRepo = movieRepo;
        }

        #region Admin Methods
        public async Task<CinemaAdminListVM> GetAllCinemasAsync(int page, int pageSize, string? query = null)
        {
            var source = _cinemaRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(c =>
                    c.Name.Contains(query) ||
                    c.Description != null && c.Description.Contains(query) ||
                    c.Location.Contains(query));
            }

            var paginatedList = await PaginatedList<Cinema>.CreateAsync(source, page, pageSize);

            return new CinemaAdminListVM
            {
                Cinemas = paginatedList.Select(c => new CinemaAdminIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Location = c.Location,
                    Img = c.ImgUrl,
                    CurrentState = c.CurrentState.Value,
                    CreatedDateUtc = c.CreatedDateUtc,
                    CinemasCount = c.CinemaMovies.Count,
                    IsDeleted = c.IsDeleted
                }),
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                TotalCount = paginatedList.TotalCount,
                SearchTerm = query
            };
        }

        public async Task<CinemaAdminDetailsVM> GetCinemaDetailsAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetAllWithDeleted()
                                .Include(m => m.CinemaMovies)
                                .ThenInclude(cm => cm.Movie)
                                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null) return null;

            return new CinemaAdminDetailsVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Description = cinema.Description,
                Location = cinema.Location,
                CurrentState = cinema.CurrentState.Value,
                CreatedBy = cinema.CreatedBy,
                CreatedDateUtc = cinema.CreatedDateUtc,
                UpdatedBy = cinema.UpdatedBy,
                UpdatedDateUtc = cinema.UpdatedDateUtc,
                DeletedBy = cinema.DeletedBy,
                DeletedDateUtc = cinema.DeletedDateUtc,
                Movies = cinema.CinemaMovies
                    .Where(cm => cm.Movie != null)          
                    .Select(cm => new MovieAdminVM
                    {
                        Id = cm.Movie.Id,
                        Title = cm.Movie.Title,
                        CurrentState = cm.Movie.CurrentState.Value,
                        IsDeleted = cm.Movie.IsDeleted,
                    }).ToList()
            };

        }

        public async Task<CinemaAdminVM?> GetByIdAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(id);
            if (cinema == null) return null;

            return new CinemaAdminVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Description = cinema.Description,
                Location = cinema.Location               
            };
        }

        public async Task<CinemaAdminVM> CreateAsync(CinemaAdminVM cinemaVM)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            string imagePath = null;
            if (cinemaVM.IconFile != null && cinemaVM.IconFile.Length > 0)
            {
                imagePath = await _fileService.SaveFileAsync(cinemaVM.IconFile, "uploads/cinemas");
            }

            var cinema = new Cinema
            {
                Name = cinemaVM.Name,
                Description = cinemaVM.Description,
                Location = cinemaVM.Location,
                ImgUrl = imagePath,
                CreatedDateUtc = DateTime.UtcNow,
                CurrentState = cinemaVM.CurrentState,
                CreatedBy = userName,
                IsDeleted = false
            };

            await _cinemaRepo.Add(cinema);

            cinemaVM.Id = cinema.Id;
            return cinemaVM;
        }

        public async Task<CinemaAdminVM> UpdateAsync(CinemaAdminVM cinemaVM)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(cinemaVM.Id);

            if (cinema == null)
                throw new Exception("Cinema not found");

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            if (cinemaVM.IconFile != null && cinemaVM.IconFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(cinema.ImgUrl))
                {
                    _fileService.DeleteFile(cinema.ImgUrl);
                }

                cinema.ImgUrl = await _fileService.SaveFileAsync(cinemaVM.IconFile, "uploads/cinemas");
            }

            cinema.Name = cinemaVM.Name;
            cinema.Description = cinemaVM.Description;
            cinema.Location = cinemaVM.Location;
            cinema.CurrentState = cinemaVM.CurrentState;
            cinema.UpdatedBy = userName;
            cinema.UpdatedDateUtc = DateTime.UtcNow;

            await _cinemaRepo.Update(cinema);
            return cinemaVM;
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(id);
            if (cinema == null)
                throw new Exception("Category not found");

            if (cinema.IsDeleted)
            {
                await _cinemaRepo.RestoreAsync(id);
            }
            else
            {
                await _cinemaRepo.SoftDeleteAsync(id);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetByIdAsync(id);
            if (cinema == null)
                throw new Exception("Cinema not found");

            if (!string.IsNullOrEmpty(cinema.ImgUrl))
            {
                try
                {
                    _fileService.DeleteFile(cinema.ImgUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not delete image file {cinema.ImgUrl}: {ex.Message}");
                }
            }

            await _cinemaRepo.DeleteInDB(id);
        }

        #endregion


        #region Customer Methods

        public async Task<IEnumerable<CinemaIndexVM>> GetActiveCinemaAsync()
        {
            try
            {
                var cinemas = await _cinemaRepo.Get(c => !c.IsDeleted && c.CurrentState.Value == DAL.Enums.CurrentState.Active)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return cinemas.Select(c => new CinemaIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Description = c.Description,
                    ImageUrl = c.ImgUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting active cinemas");
                throw;
            }
        }

        public async Task<CinemaDetailsVM?> GetCinemaDetailsAsync(Guid id, string? searchString = null, string? sortOrder = null)
        {
            try
            {
                var cinema = await _cinemaRepo.GetAll()
                    .Include(c => c.CinemaMovies)
                        .ThenInclude(cm => cm.Movie)
                        .ThenInclude(m => m.Category)
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (cinema == null)
                    return null;

                var cinemaDetails = new CinemaDetailsVM
                {
                    Id = cinema.Id,
                    Name = cinema.Name,
                    Location = cinema.Location,
                    Description = cinema.Description,
                    CreatedDate = cinema.CreatedDateUtc,
                    Img = cinema.ImgUrl
                };

                var moviesQuery = _movieRepo.GetAll()
                    .Where(m => m.CinemaMovies.Any(cm => cm.CinemaId == id && !cm.IsDeleted))
                    .Include(m => m.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    moviesQuery = moviesQuery.Where(m =>
                        m.Title.Contains(searchString) ||
                        (m.Description != null && m.Description.Contains(searchString)));
                }

                moviesQuery = sortOrder switch
                {
                    "title" => moviesQuery.OrderBy(m => m.Title),
                    "title_desc" => moviesQuery.OrderByDescending(m => m.Title),
                    "date" => moviesQuery.OrderByDescending(m => m.StartDate),
                    "date_desc" => moviesQuery.OrderBy(m => m.EndDate),
                    "rating" => moviesQuery.OrderByDescending(m => m.Rating),
                    _ => moviesQuery.OrderBy(m => m.Title)
                };

                cinemaDetails.Movies = await moviesQuery
                    .Select(m => new MovieDetailsVM
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        Price = m.Price,
                        Author = m.Author,
                        ImgUrl = m.ImgUrl,
                        Duration = m.Duration,
                        StartDate = m.StartDate,
                        EndDate = m.EndDate,
                        ReleaseYear = m.ReleaseYear,
                        Rating = m.Rating,
                        CategoryName = m.Category.Name
                    })
                    .ToListAsync();

                return cinemaDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting cinema details for ID: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<CinemaIndexVM>> GetPopularCinemaAsync(int count)
        {
            try
            {
                // This is a simplified version - you might want to implement actual popularity logic
                var cinemas = await _cinemaRepo.GetAll()
                    .OrderByDescending(c => c.CreatedDateUtc)
                    .Take(count)
                    .ToListAsync();

                return cinemas.Select(c => new CinemaIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Location = c.Location,
                    Description = c.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting top {count} popular cinemas");
                throw;
            }
        }

        #endregion


    }
}
