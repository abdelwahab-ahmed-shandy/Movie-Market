using DAL.ViewModels.Category;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        #region Admin Category Service Methods

        Task<CategoryAdminListVM> GetAllCategoriesAsync(int page, int pageSize, string query = null);
        Task<CategoryAdminDetailsVM?> GetCategoryDetailsAsync(Guid id);
        Task<Category> CreateCategoryAsync(CategoryAdminCreateEditVM viewModel);
        Task UpdateCategoryAsync(Guid id, CategoryAdminCreateEditVM viewModel);
        Task SoftDelete(Guid id);
        Task DeleteAsync(Guid id);
        #endregion

        #region Customer Category Service Methods

        Task<IEnumerable<CategoryIndexVM>> GetActiveCategoriesAsync();
        Task<CategoryDetailsVM?> GetCategoryWithMoviesAsync(Guid id);
        Task<IEnumerable<CategoryIndexVM>> GetPopularCategoriesAsync(int count);

        #endregion

    }
}
