using DAL.ViewModels.TvSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Admin
{
    public interface IAdminTvSeriesService
    {
        Task<TvSeriesAdminListVM> GetAllTvSeriesAsync(int page, int pageSize, string? query = null);
        Task<TvSeriesAdminDetailsVM> GetTvSeriesDetailsAsync(Guid id);
        Task<TvSeries> CreateTvSeriesAsync(TvSeriesAdminCreateVM viewModel);
        Task UpdateTvSeriesAsync(Guid id, TvSeriesAdminEditVM viewModel);
        Task RestoreAsync(Guid Id);
        Task SoftDelete(Guid Id);
        Task Delete(Guid Id);
    }
}
