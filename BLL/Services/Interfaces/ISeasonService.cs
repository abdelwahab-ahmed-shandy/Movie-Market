using DAL.ViewModels.Season;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISeasonService 
    {

        #region Admin Methods
        Task<SeasonAdminListVM> GetAllSeasonsAsync(int page, int pageSize, Guid tvSeriesId, string? query = null);
        Task<SeasonAdminDetailsVM> GetSeasonDetailsAsync(Guid id);
        Task<Season> CreateSeasonAsync(SeasonAdminCreateVM viewModel);
        Task UpdateSeasonAsync(Guid id, SeasonAdminEditVM viewModel);
        Task SoftDeleteSeasonAsync(Guid id);
        Task DeleteSeasonAsync(Guid id);
        Task RestoreSeasonAsync(Guid id);
        #endregion

        #region Customer Methods
        Task<List<SeasonCustomerVM>> GetAllSeasonAsync();
        Task<SeasonDetailsVM?> GetSeasonWithDetailsAsync(Guid id);
        #endregion
  
    }
}
