using BLL.Services.Interfaces.Admin;
using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminCategoryService(IGenericRepository<Category> categoryRepository,
                                        IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepo = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CategoryAdminListVM> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10, string searchTerm = null)
        {
            var query = _categoryRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    c.Description != null && c.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var categories = await query
                .Select(c => new CategoryAdminIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CurrentState = c.CurrentState.Value,
                    CreatedDateUtc = c.CreatedDateUtc,
                    MoviesCount = c.Movies.Count,
                    IsDeleted = c.IsDeleted
                })
                .OrderByDescending(c => c.CreatedDateUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new CategoryAdminListVM
            {
                Categories = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };
        }


        public async Task<CategoryAdminDetailsVM?> GetCategoryDetailsAsync(Guid id)
        {
            var category = await _categoryRepo.GetAll()
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return null;

            return new CategoryAdminDetailsVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CurrentState = category.CurrentState.Value,
                CreatedBy = category.CreatedBy,
                CreatedDateUtc = category.CreatedDateUtc,
                UpdatedBy = category.UpdatedBy,
                UpdatedDateUtc = category.UpdatedDateUtc,
                DeletedBy = category.DeletedBy,
                DeletedDateUtc = category.DeletedDateUtc,
                Movies = category.Movies.Select(m => new MovieAdminVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    CurrentState = m.CurrentState.Value,
                    IsDeleted = m.IsDeleted
                })
            };
        }


        public async Task<Category> CreateCategoryAsync(CategoryAdminCreateEditVM vM)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var category = new Category
            {
                Name = vM.Name,
                Description = vM.Description,
                CurrentState = vM.CurrentState,
                CreatedBy = userName,
                CreatedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _categoryRepo.Add(category);
        }


        public async Task UpdateCategoryAsync(Guid id, CategoryAdminCreateEditVM vM)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                throw new Exception("Category not found");

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            category.Name = vM.Name;
            category.Description = vM.Description;
            category.CurrentState = vM.CurrentState;
            category.UpdatedBy = userName;
            category.UpdatedDateUtc = DateTime.UtcNow;

            await _categoryRepo.Update(category);
        }


        public async Task SoftDelete(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            if (category.IsDeleted)
            {
                await _categoryRepo.RestoreAsync(id);
            }
            else
            {
                await _categoryRepo.SoftDeleteAsync(id);
            }
        }

        public async Task Delete(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            await _categoryRepo.DeleteInDB(id);
        }


    }
}
