using DAL.ViewModels.Episode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IEpisodeService
    {
        Task<EpisodeAdminListVM> GetAllEpisodesAsync(int page, int pageSize, Guid seasonId, string? query = null);
        Task<EpisodeAdminDetailsVM> GetEpisodeDetailsAsync(Guid id);
        Task<Episode> CreateEpisodeAsync(EpisodeAdminCreateVM viewModel);
        Task UpdateEpisodeAsync(Guid id, EpisodeAdminEditVM viewModel);
        Task SoftDeleteEpisodeAsync(Guid id);
        Task DeleteEpisodeAsync(Guid id);
        Task RestoreEpisodeAsync(Guid id);
    }
}
