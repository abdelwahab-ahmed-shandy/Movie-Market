
namespace BLL.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchAdminResultVM> GlobalSearchAdminAsync(string query);
        Task<SearchCutomerResultVM> GlobalSearchCustomerAsync(string query);

    }
}
