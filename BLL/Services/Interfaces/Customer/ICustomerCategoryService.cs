using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerCategoryService
    {
        Task<IEnumerable<CategoryIndexVM>> GetActiveCategoriesAsync();
        Task<CategoryDetailsVM?> GetCategoryWithMoviesAsync(Guid id);
        Task<IEnumerable<CategoryIndexVM>> GetPopularCategoriesAsync(int count);
    }
}
