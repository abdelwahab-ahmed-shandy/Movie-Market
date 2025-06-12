using BLL.Services.Interfaces.Admin;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminSpecialService : IAdminSpecialService
    {
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly ILogger<AdminSpecialService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminSpecialService(IGenericRepository<Special> specialRepo,
                                        ILogger<AdminSpecialService> logger,
                                        IHttpContextAccessor httpContextAccessor)
        {
            _specialRepo = specialRepo;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SpecialAdminListVM> GetAllSpecialsAsync(int page, int pageSize, string? query = null)
        {
            try
            {
                var source = _specialRepo.GetAllWithDeleted();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.Trim().ToLower();

                    source = source.Where(s =>
                        s.Name.ToLower().Contains(query) ||
                        (s.Description != null && s.Description.ToLower().Contains(query)));
                }


                source = source.OrderByDescending(s => s.CreatedDateUtc);

                var paginatedList = await PaginatedList<Special>.CreateAsync(source, page, pageSize);

                return new SpecialAdminListVM
                {
                    Specials = paginatedList.Select(s => new SpecialAdminVM
                    {
                        Id = s.Id,
                        Name = s.Name,
                        DiscountPercentage = s.DiscountPercentage,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        IsActive = s.CurrentState == CurrentState.Active &&
                                   !s.IsDeleted &&
                                   DateTime.Now >= s.StartDate &&
                                   DateTime.Now <= s.EndDate
                    }).ToList(),
                    PageNumber = paginatedList.PageIndex,
                    PageSize = pageSize,
                    TotalCount = paginatedList.TotalCount,
                    SearchTerm = query
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving specials list");
                throw;
            }
        }

        public async Task<SpecialAdminDetailsVM?> GetSpecialByIdAsync(Guid id)
        {
            var special = await _specialRepo.GetAllWithDeleted()
                .Include(s => s.MovieSpecials)
                .ThenInclude(ms => ms.ParentMovie)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (special == null) return null;

            return new SpecialAdminDetailsVM
            {
                Id = special.Id,
                Name = special.Name,
                Description = special.Description,
                DiscountPercentage = special.DiscountPercentage,
                StartDate = special.StartDate,
                EndDate = special.EndDate,
                CurrentState = special.CurrentState.Value,
                Movies = special.MovieSpecials
                    .OrderBy(ms => ms.DisplayOrder)
                    .Select(ms => new SpecialMovieItemVM
                    {
                        MovieId = ms.ParentMovieId,
                        Title = ms.ParentMovie.Title,
                        IsFeatured = ms.IsFeatured,
                        DisplayOrder = ms.DisplayOrder
                    }).ToList()
            };
        }


        public async Task<Guid> CreateSpecialAsync(SpecialCreateVM model)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var special = new Special
            {
                Name = model.Name,
                Description = model.Description,
                DiscountPercentage = model.DiscountPercentage,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CurrentState = model.IsActive ? CurrentState.Active : CurrentState.Inactive,
                CreatedBy = userName,
                CreatedDateUtc = DateTime.UtcNow
            };

            await _specialRepo.Add(special);
            return special.Id;
        }


        public async Task<bool> UpdateSpecialAsync(Guid id, SpecialEditVM model)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var special = await _specialRepo.GetByIdAsync(id);
            if (special == null) return false;

            special.Name = model.Name;
            special.Description = model.Description;
            special.DiscountPercentage = model.DiscountPercentage;
            special.StartDate = model.StartDate;
            special.EndDate = model.EndDate;
            special.CurrentState = model.IsActive ? CurrentState.Active : CurrentState.Inactive;
            special.UpdatedDateUtc = DateTime.UtcNow;
            special.UpdatedBy = userName;

            await _specialRepo.Update(special);
            return true;
        }


        public async Task<bool> ToggleSpecialStatusAsync(Guid id)
        {
            var special = await _specialRepo.GetByIdAsync(id);
            if (special == null) return false;

            special.CurrentState = special.CurrentState == CurrentState.Active
                ? CurrentState.Inactive
                : CurrentState.Active;

            await _specialRepo.Update(special);
            return true;
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var cinema = await _specialRepo.GetByIdAsync(id);
            if (cinema == null)
                throw new Exception("Category not found");

            if (cinema.IsDeleted)
            {
                await _specialRepo.RestoreAsync(id);
            }
            else
            {
                await _specialRepo.SoftDeleteAsync(id);
            }
        }


        public async Task DeleteAsync(Guid id)
        {
            var category = await _specialRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            await _specialRepo.DeleteInDB(id);
        }


    }
}
