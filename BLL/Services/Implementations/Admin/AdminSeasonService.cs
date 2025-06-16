using BLL.Services.Interfaces.Admin;
using DAL.ViewModels.Season;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Admin
{
    public class AdminSeasonService : IAdminSeasonService
    {
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepository;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public AdminSeasonService(IGenericRepository<Season> seasonRepository,
                                    IGenericRepository<TvSeries> tvSeriesRepository,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _seasonRepository = seasonRepository;
            _tvSeriesRepository = tvSeriesRepository;
            _HttpContextAccessor = httpContextAccessor;
        }

        public async Task<SeasonAdminListVM> GetAllSeasonsAsync(int page, int pageSize, Guid tvSeriesId, string? query = null)
        {
            var seasonsQuery = _seasonRepository.GetAllWithDeleted()
                .Include(s => s.TvSeries)
                .Include(s => s.Episodes)
                .Where(s => s.TvSeriesId == tvSeriesId);

            if (!string.IsNullOrEmpty(query))
            {
                seasonsQuery = seasonsQuery.Where(s => s.Title.Contains(query) );
            }

            var paginatedList = await PaginatedList<Season>.CreateAsync(seasonsQuery, page, pageSize);

            return new SeasonAdminListVM
            {
                Seasons = paginatedList.Select(s => new SeasonAdminVM
                {
                    Id = s.Id,
                    Title = s.Title,
                    SeasonNumber = s.SeasonNumber,
                    ReleaseYear = s.ReleaseYear,
                    EpisodeCount = s.Episodes.Count(e => !e.IsDeleted),
                    TvSeriesTitle = s.TvSeries.Title,
                    IsDeleted = s.IsDeleted,
                    CurrentState = s.CurrentState.Value
                }).ToList(),
                TotalCount = paginatedList.TotalCount,
                PageNumber = paginatedList.PageIndex,
                PageSize = pageSize,
                SearchTerm = query,
                TvSeriesId = tvSeriesId
            };
        }


        public async Task<SeasonAdminDetailsVM> GetSeasonDetailsAsync(Guid id)
        {
            var season = await _seasonRepository.GetAllWithDeleted()
                .Include(s => s.TvSeries)
                .Include(s => s.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (season == null)
            {
                throw new KeyNotFoundException("Season not found");
            }

            var episodesVM = season.Episodes
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.EpisodeNumber)
                .Select(e => new EpisodeAdminVM
                {
                    Id = e.Id,
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    IsDeleted = e.IsDeleted
                }).ToList();

            return new SeasonAdminDetailsVM
            {
                Id = season.Id,
                Title = season.Title,
                SeasonNumber = season.SeasonNumber,
                ReleaseYear = season.ReleaseYear,
                IsDeleted = season.IsDeleted,
                CurrentState = season.CurrentState.Value,
                TvSeriesTitle = season.TvSeries.Title,
                TvSeriesId = season.TvSeriesId,
                Episodes = episodesVM,
                CreatedDateUtc = season.CreatedDateUtc,
                CreatedBy = season.CreatedBy,
                LastModifiedDateUtc = season.UpdatedDateUtc,
                LastModifiedBy = season.UpdatedBy
            };
        }


        public async Task<Season> CreateSeasonAsync(SeasonAdminCreateVM viewModel)
        {
            var userName = _HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var tvSeries = await _tvSeriesRepository.GetByIdAsync(viewModel.TvSeriesId);
            if (tvSeries == null)
            {
                throw new KeyNotFoundException("TV Series not found");
            }

            var season = new Season
            {
                Title = viewModel.Title,
                SeasonNumber = viewModel.SeasonNumber,
                ReleaseYear = viewModel.ReleaseYear,
                TvSeriesId = viewModel.TvSeriesId,
                CreatedBy = userName,
                CreatedDateUtc = DateTime.UtcNow
            };

            return await _seasonRepository.Add(season);
        }


        public async Task UpdateSeasonAsync(Guid id, SeasonAdminEditVM viewModel)
        {
            var userName = _HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var season = await _seasonRepository.GetByIdAsync(id);
            if (season == null)
            {
                throw new KeyNotFoundException("Season not found");
            }

            season.Title = viewModel.Title;
            season.SeasonNumber = viewModel.SeasonNumber;
            season.ReleaseYear = viewModel.ReleaseYear;
            season.CurrentState = viewModel.CurrentState;
            season.UpdatedBy = userName;
            season.UpdatedDateUtc = DateTime.UtcNow;

            await _seasonRepository.Update(season);
        }


        public async Task SoftDeleteSeasonAsync(Guid id)
        {
            await _seasonRepository.SoftDeleteAsync(id);
        }


        public async Task DeleteSeasonAsync(Guid id)
        {
            await _seasonRepository.DeleteInDB(id);
        }


        public async Task RestoreSeasonAsync(Guid id)
        {
            await _seasonRepository.RestoreAsync(id);
        }
    
    
    }
}