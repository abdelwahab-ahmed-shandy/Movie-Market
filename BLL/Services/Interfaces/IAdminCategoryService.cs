using DAL.ViewModels.Category;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAdminCategoryService
    {
        Task<IEnumerable<CategoryAdminIndexVM>> GetAllCategoriesAsync();
        Task<CategoryAdminDetailsVM?> GetCategoryDetailsAsync(Guid id);
        Task<Category> CreateCategoryAsync(CategoryAdminCreateEditVM viewModel);
        Task UpdateCategoryAsync(Guid id, CategoryAdminCreateEditVM viewModel);
        Task ToggleCategoryStatusAsync(Guid id);
        Task DeleteCategoryPermanentlyAsync(Guid id);
    }
}
