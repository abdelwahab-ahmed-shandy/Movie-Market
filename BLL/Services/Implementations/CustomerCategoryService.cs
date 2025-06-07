using DAL.Enums;
using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CustomerCategoryService : ICustomerCategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CustomerCategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryIndexVM>> GetActiveCategoriesAsync()
        {
            return await _categoryRepository.GetAll()
                .Where(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    MoviesCount = c.Movies.Count(m => !m.IsDeleted && m.CurrentState == CurrentState.Active)
                }).ToListAsync();
        }

        public async Task<CategoryDetailsVM?> GetCategoryWithMoviesAsync(Guid id)
        {
            var category = await _categoryRepository.GetAll()
                .Include(c => c.Movies.Where(m => !m.IsDeleted && m.CurrentState == CurrentState.Active))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return null;

            return new CategoryDetailsVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Movies = category.Movies.Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Title,
                    PosterUrl = m.ImgUrl,
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
            return await _categoryRepository.GetAll()
                .Where(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
                .OrderByDescending(c => c.Movies.Count)
                .Take(count)
                .Select(c => new CategoryIndexVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    MoviesCount = c.Movies.Count
                }).ToListAsync();
        }


    }
}
