﻿
namespace BLL.Services.Interfaces
{
    public interface ITvSeriesService
    {

        #region Admin Method
        Task<TvSeriesAdminListVM> GetAllTvSeriesAsync(int page, int pageSize, string? query = null);
        Task<TvSeriesAdminDetailsVM> GetTvSeriesDetailsAsync(Guid id);
        Task<TvSeries> CreateTvSeriesAsync(TvSeriesAdminCreateVM viewModel);
        Task UpdateTvSeriesAsync(Guid id, TvSeriesAdminEditVM viewModel);
        Task RestoreAsync(Guid Id);
        Task SoftDelete(Guid Id);
        Task Delete(Guid Id);
        #endregion

        #region Customer

        Task<IEnumerable<TvSeriesCharacterVM>> GetAllTvSeriesAsync();
        Task<TvSeriesDetailsVM?> GetTvSeriesWithDetailsAsync(Guid id);

        #endregion

    }
}
