
namespace BLL.Services.Implementations
{
    public class SeasonService : ISeasonService
    {
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepository;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly ILogger<SeasonService> _logger;

        public SeasonService(IGenericRepository<Season> seasonRepository,
                                    IGenericRepository<TvSeries> tvSeriesRepository,
                                    IHttpContextAccessor httpContextAccessor, ILogger<SeasonService> logger)
        {
            _seasonRepository = seasonRepository;
            _logger = logger;
            _tvSeriesRepository = tvSeriesRepository;
            _HttpContextAccessor = httpContextAccessor;
        }

        #region Admin Methods

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
                .Select(e => new EpisodeSeasonAdminVM
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
        #endregion
         

        #region Customer Methods
        public async Task<List<SeasonCustomerVM>> GetAllSeasonAsync()
        {
            var seasons = await _seasonRepository.GetAll()
                .Include(s => s.TvSeries)
                .Include(s => s.Episodes.Where(e => !e.IsDeleted))
                .OrderBy(s => s.SeasonNumber)
                .ToListAsync();

            return seasons.Select(s => new SeasonCustomerVM
            {
                Id = s.Id,
                Title = s.Title,
                SeasonNumber = s.SeasonNumber,
                TvSeriesTitle = s.TvSeries.Title,
                ImgUrl = s.TvSeries.ImgUrl,
                ReleaseYear = s.ReleaseYear,
                EpisodeCount = s.Episodes.Count
            }).ToList();
        }

        public async Task<SeasonDetailsVM?> GetSeasonWithDetailsAsync(Guid id)
        {
            try
            {
                var season = await _seasonRepository.GetAll()
                    .Include(s => s.TvSeries)
                    .Include(s => s.Episodes.Where(e => !e.IsDeleted))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

                if (season == null)
                {
                    return null;
                }

                // Map episodes
                var episodes = season.Episodes
                    .OrderBy(e => e.EpisodeNumber)
                    .Select(e => new EpisodeCustomerVM
                    {
                        Id = e.Id,
                        Title = e.Title,
                        EpisodeNumber = e.EpisodeNumber,
                        Description = e.Description,
                        Duration = e.Duration,
                        ThumbnailUrl = season.TvSeries.ImgUrl
                    }).ToList();

                var tvSeriesVMs = new List<TvSeriesCharacterVM>
                {
                new TvSeriesCharacterVM
                {
                Id = season.TvSeries.Id,
                Title = season.TvSeries.Title,
                Description = season.TvSeries.Description,
                Author = season.TvSeries.Author,
                ImgUrl = season.TvSeries.ImgUrl,
                ReleaseYear = season.TvSeries.ReleaseYear,
                Rating = season.TvSeries.Rating
                 }
                };

                return new SeasonDetailsVM
                {
                    Id = season.Id,
                    Title = season.Title,
                    SeasonNumber = season.SeasonNumber,
                    ReleaseYear = season.ReleaseYear,
                    TvSeriesTitle = season.TvSeries.Title,
                    TvSeriesId = season.TvSeries.Id,
                    tvSeriesVMs = tvSeriesVMs,
                    Episodes = episodes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving season details for ID: {id}");
                throw;
            }
        }

        #endregion


    }
}