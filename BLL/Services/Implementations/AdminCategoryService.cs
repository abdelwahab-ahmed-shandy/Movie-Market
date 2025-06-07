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
        private readonly IMapper _mapper;
        public AdminCategoryService(IGenericRepository<Category> categoryRepository,
                                        IHttpContextAccessor httpContextAccessor,
                                            IMapper mapper)
        {
            _categoryRepo = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryAdminIndexVM>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAll().Select(c => new CategoryAdminIndexVM
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

        public async Task<Category> CreateCategoryAsync(CategoryAdminCreateEditVM viewModel)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
            });

            var mapper = config.CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            var category = mapper.Map<Category>(viewModel, opts =>
            {
                opts.Items["HttpContextAccessor"] = _httpContextAccessor;
            });

            return await _categoryRepo.Add(category);
        }


        public async Task UpdateCategoryAsync(Guid id, CategoryAdminCreateEditVM viewModel)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
            });

            var mapper = config.CreateMapper();
            mapper.Map(viewModel, category, opts =>
            {
                opts.Items["HttpContextAccessor"] = _httpContextAccessor;
            });

            await _categoryRepo.Update(category);
        }

        public async Task ToggleCategoryStatusAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            if (category.IsDeleted)
            {
                await _categoryRepo.RestoreAsync(id);
            }
            else
            {
                await _categoryRepo.SoftDeleteAsync(id);
            }
        }


    }
}
