using BLL.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
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

        public AdminCinemaService(IGenericRepository<Cinema> cinemaRepo, IHttpContextAccessor httpContextAccessor)
        {
            _cinemaRepo = cinemaRepo;
            _httpContextAccessor = httpContextAccessor;

        }


        public async Task<CinemaAdminListVM> GetAllCinemasAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var query = _cinemaRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    c.Description != null && c.Description.Contains(searchTerm) ||
                    c.Location.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var cinemas = await query
                .Select(c => new CinemaAdminIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Location = c.Location,
                    CurrentState = c.CurrentState.Value,
                    CreatedDateUtc = c.CreatedDateUtc,
                    CinemasCount = c.CinemaMovies.Count,
                    IsDeleted = c.IsDeleted
                })
                .OrderByDescending(c => c.CreatedDateUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new CinemaAdminListVM
            {
                Cinemas = cinemas,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };
        }


        public async Task<CinemaAdminDetailsVM> GetCinemaDetailsAsync(Guid id)
        {
            var cinema = await _cinemaRepo.GetAll()
                                .Include(m => m.CinemaMovies)
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
                Movies = cinema.CinemaMovies.Select(cm => new MovieAdminVM
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

            var cinema = new Cinema
            {
                Name = cinemaVM.Name,
                Description = cinemaVM.Description,
                Location = cinemaVM.Location,
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
            var category = await _cinemaRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            await _cinemaRepo.DeleteInDB(id);
        }


    }
}
