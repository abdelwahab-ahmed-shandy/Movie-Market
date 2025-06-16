using BLL.Services.Interfaces.Admin;
using DAL.ViewModels.Episode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminEpisodeService : IAdminEpisodeService
    {
        private readonly IGenericRepository<Episode> _episodeRepository;
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminEpisodeService(IGenericRepository<Episode> episodeRepository,
            IGenericRepository<Season> seasonRepository , IHttpContextAccessor httpContextAccessor)
        {
            _episodeRepository = episodeRepository;
            _seasonRepository = seasonRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<EpisodeAdminListVM> GetAllEpisodesAsync(int page, int pageSize, Guid seasonId, string? query = null)
        {
            var source = _episodeRepository.GetAllWithDeleted()
                .Include(e => e.Season)
                .ThenInclude(s => s.TvSeries)
                .Where(e => e.SeasonId == seasonId);

            if (!string.IsNullOrEmpty(query))
            {
                source = source.Where(e => e.Title.Contains(query) || e.Description.Contains(query));
            }

            var paginatedList = await PaginatedList<Episode>.CreateAsync(source, page, pageSize);

            return new EpisodeAdminListVM
            {
                Episodes = paginatedList.Select(e => new EpisodeAdminVM
                {
                    Id = e.Id,
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    SeasonTitle = e.Season.Title,
                    TvSeriesTitle = e.Season.TvSeries.Title,
                    Duration = e.Duration,
                    Rating = e.Rating,
                    IsDeleted = e.IsDeleted,
                    CurrentState = e.CurrentState.Value
                }).ToList(),
                TotalCount = paginatedList.TotalCount,
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                SearchTerm = query,
                SeasonId = seasonId
            };
        }


        public async Task<EpisodeAdminDetailsVM> GetEpisodeDetailsAsync(Guid id)
        {
            var episode = await _episodeRepository.GetAllWithDeleted()
                .Include(e => e.Season)
                .ThenInclude(s => s.TvSeries)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                throw new KeyNotFoundException("Episode not found");
            }

            return new EpisodeAdminDetailsVM
            {
                Id = episode.Id,
                Title = episode.Title,
                EpisodeNumber = episode.EpisodeNumber,
                Description = episode.Description,
                Rating = episode.Rating,
                Duration = episode.Duration,
                VideoUrl = episode.VideoUrl,
                ThumbnailUrl = episode.ThumbnailUrl,
                IsDeleted = episode.IsDeleted,
                CurrentState = episode.CurrentState.Value,
                SeasonTitle = episode.Season.Title,
                SeasonId = episode.SeasonId,
                TvSeriesTitle = episode.Season.TvSeries.Title,
                TvSeriesId = episode.Season.TvSeries.Id,
                CreatedDateUtc = episode.CreatedDateUtc,
                CreatedBy = episode.CreatedBy,
                LastModifiedDateUtc = episode.UpdatedDateUtc,
                LastModifiedBy = episode.UpdatedBy
            };
        }


        public async Task<Episode> CreateEpisodeAsync(EpisodeAdminCreateVM viewModel)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var season = await _seasonRepository.GetByIdAsync(viewModel.SeasonId);
            if (season == null)
            {
                throw new KeyNotFoundException("Season not found");
            }

            var episode = new Episode
            {
                EpisodeNumber = viewModel.EpisodeNumber,
                Title = viewModel.Title,
                Description = viewModel.Description,
                Rating = viewModel.Rating,
                Duration = viewModel.Duration,
                VideoUrl = viewModel.VideoUrl,
                ThumbnailUrl = viewModel.ThumbnailUrl,
                SeasonId = viewModel.SeasonId,
                CreatedBy = userName,
                CreatedDateUtc = DateTime.UtcNow,
            };

            return await _episodeRepository.Add(episode);
        }


        public async Task UpdateEpisodeAsync(Guid id, EpisodeAdminEditVM viewModel)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var episode = await _episodeRepository.GetByIdAsync(id);
            if (episode == null)
            {
                throw new KeyNotFoundException("Episode not found");
            }

            episode.EpisodeNumber = viewModel.EpisodeNumber;
            episode.Title = viewModel.Title;
            episode.Description = viewModel.Description;
            episode.Rating = viewModel.Rating;
            episode.Duration = viewModel.Duration;
            episode.VideoUrl = viewModel.VideoUrl;
            episode.ThumbnailUrl = viewModel.ThumbnailUrl;
            episode.CurrentState = viewModel.CurrentState;
            episode.UpdatedBy = userName;
            episode.UpdatedDateUtc = DateTime.UtcNow;

            await _episodeRepository.Update(episode);
        }


        public async Task SoftDeleteEpisodeAsync(Guid id)
        {
            await _episodeRepository.SoftDeleteAsync(id);
        }


        public async Task DeleteEpisodeAsync(Guid id)
        {
            await _episodeRepository.DeleteInDB(id);
        }


        public async Task RestoreEpisodeAsync(Guid id)
        {
            await _episodeRepository.RestoreAsync(id);
        }


    }
}
