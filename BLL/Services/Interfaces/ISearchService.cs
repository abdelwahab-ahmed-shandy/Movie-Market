using DAL.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchAdminResultVM> GlobalSearchAdminAsync(string query);
        Task<SearchCutomerResultVM> GlobalSearchCustomerAsync(string query);

    }
}
