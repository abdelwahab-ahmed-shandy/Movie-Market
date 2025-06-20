using BLL.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminCinemaService : IAdminCinemaService
    {
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly ILogger<AdminCinemaService> _logger;
        public AdminCinemaService(IGenericRepository<Cinema> cinemaRepo, IHttpContextAccessor httpContextAccessor,
                                    IFileService fileService , ILogger<AdminCinemaService> logger)
        {
            _cinemaRepo = cinemaRepo;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _logger = logger;
        }



        public async Task<CinemaAdminListVM> GetAllCinemasAsync(int page, int pageSize, string? query = null)
        {
            var source = _cinemaRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(c =>
                    c.Name.Contains(query) ||
                    (c.Description != null && c.Description.Contains(query)) ||
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


    }
}
