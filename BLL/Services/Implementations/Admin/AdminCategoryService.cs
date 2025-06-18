using BLL.Services.Interfaces.Admin;
using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IFileService _fileService;
        private readonly ILogger<AdminCategoryService> _logger;
        public AdminCategoryService(IGenericRepository<Category> categoryRepository,
                                        IHttpContextAccessor httpContextAccessor ,
                                        IFileService fileService,
                                        ILogger<AdminCategoryService> logger)
        {
            _categoryRepo = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<CategoryAdminListVM> GetAllCategoriesAsync(int page, int pageSize, string? query = null)
        {
            var source = _categoryRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(c =>
                    c.Name.Contains(query) ||
                    (c.Description != null && c.Description.Contains(query)));
            }

            var paginatedList = await PaginatedList<Category>.CreateAsync(source, page, pageSize);

            return new CategoryAdminListVM
            {
                Categories = paginatedList.Select(c => new CategoryAdminIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconUrl = c.ImgUrl,
                    Description = c.Description,
                    CurrentState = c.CurrentState.Value,
                    CreatedDateUtc = c.CreatedDateUtc,
                    MoviesCount = c.Movies.Count,
                    IsDeleted = c.IsDeleted
                }),
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                TotalCount = paginatedList.TotalCount,
                SearchTerm = query
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
                IconUrl = category.ImgUrl,
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

            string imagePath = null;
            if (vM.IconFile != null && vM.IconFile.Length > 0)
            {
                imagePath = await _fileService.SaveFileAsync(vM.IconFile, "uploads/categories");
            }

            return await _categoryRepo.Add(new Category
            {
                Name = vM.Name,
                Description = vM.Description,
                ImgUrl = imagePath,
                CurrentState = vM.CurrentState,
                CreatedBy = userName,
                CreatedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            });
        }


        public async Task UpdateCategoryAsync(Guid id, CategoryAdminCreateEditVM vM)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                throw new Exception("Category not found");

            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            if (vM.IconFile != null && vM.IconFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(category.ImgUrl))
                {
                    _fileService.DeleteFile(category.ImgUrl);
                }

                category.ImgUrl = await _fileService.SaveFileAsync(vM.IconFile, "uploads/categories");
            }

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


        public async Task DeleteAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Category not found");

            if (!string.IsNullOrEmpty(category.ImgUrl))
            {
                try
                {
                    _fileService.DeleteFile(category.ImgUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not delete image file {category.ImgUrl}: {ex.Message}");
                }
            }

            await _categoryRepo.DeleteInDB(id);
        }


    }
}
