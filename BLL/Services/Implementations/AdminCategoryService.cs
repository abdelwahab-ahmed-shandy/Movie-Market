using BLL.Services.Interfaces.Mappings;
using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminCategoryService(IGenericRepository<Category> categoryRepository,
                                        IHttpContextAccessor httpContextAccessor
                                            )
        {
            _categoryRepo = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CategoryAdminIndexVM>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAllWithDeleted().Select(c => new CategoryAdminIndexVM
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
                .ToListAsync();
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


        public async Task ToggleCategoryStatusAsync(Guid id)
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

        public async Task DeleteCategoryPermanentlyAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            await _categoryRepo.DeleteInDB(id);
        }

    }
}
