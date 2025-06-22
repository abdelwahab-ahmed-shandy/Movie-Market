using DAL.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IDashboardService
    {

        #region Admin Methods

        Task<List<Subscriber>> GetTotalSubscriberAsync(int count);
        Task<AdminDashboardVM> GetAdminDashboardDataAsync();
        Task<int> GetTotalActiveSpecialsAsync();
        Task<int> GetTotalCharactersAsync();
        Task<int> GetTotalCategoriesAsync();
        Task<int> GetTotalMoviesAsync();
        Task<int> GetTotalCinemasAsync();
        Task<int> GetTotalTvSeriesAsync();
        Task<int> GetTotalOrdersAsync();
        //Task<int> GetTotalUsersAsync();

        #endregion

        #region Customer Methods
        Task<DashboardVM> GetDashboardDataAsync();
        #endregion

    }
}
