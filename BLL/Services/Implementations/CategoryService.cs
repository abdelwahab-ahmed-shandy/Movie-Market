using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(IGenericRepository<Category> categoryRepository,
                                        IHttpContextAccessor httpContextAccessor ,
                                        IFileService fileService,
                                        ILogger<CategoryService> logger)
        {
            _categoryRepo = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _logger = logger;
        }

        #region Admin Category Service Methods

        public async Task<CategoryAdminListVM> GetAllCategoriesAsync(int page, int pageSize, string? query = null)
        {
            var source = _categoryRepo.GetAllWithDeleted();

            if (!string.IsNullOrWhiteSpace(query))
            {
                source = source.Where(c =>
                    c.Name.Contains(query) ||
                    c.Description != null && c.Description.Contains(query));
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

        #endregion


        #region Customer Category Service Methods

        public async Task<IEnumerable<CategoryIndexVM>> GetActiveCategoriesAsync()
        {
            return await _categoryRepo.Get(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
                .Select(c => new CategoryIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.ImgUrl,
                    MoviesCount = c.Movies.Count(m => !m.IsDeleted && m.CurrentState == CurrentState.Active)
                }).ToListAsync();
        }

        public async Task<CategoryDetailsVM?> GetCategoryWithMoviesAsync(Guid id)
        {
            var category = await _categoryRepo.GetAll()
                .Include(c => c.Movies.Where(m => !m.IsDeleted && m.CurrentState == CurrentState.Active))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return null;

            return new CategoryDetailsVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                //IconUrl = category.ImgUrl,
                Movies = category.Movies.Select(m => new MovieCharacterIndexVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    ImgUrl = m.ImgUrl,
                    ReleaseYear = m.ReleaseYear,
                    Rating = m.Rating,
                    ShortDescription = m.Description?.Length > 100 ?
                        m.Description.Substring(0, 100) + "..." :
                        m.Description
                })
            };
        }

        public async Task<IEnumerable<CategoryIndexVM>> GetPopularCategoriesAsync(int count)
        {
            return await _categoryRepo.GetAll()
                .Where(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
                .OrderByDescending(c => c.Movies.Count)
                .Take(count)
                .Select(c => new CategoryIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconUrl = c.ImgUrl,
                    MoviesCount = c.Movies.Count
                }).ToListAsync();
        }

        #endregion

    }
}
