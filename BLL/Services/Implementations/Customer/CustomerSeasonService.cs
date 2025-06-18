using DAL.ViewModels.Season;
using DAL.ViewModels.TvSeries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Customer
{
    public class CustomerSeasonService : ICustomerSeasonService
    {
        private readonly IGenericRepository<Season> _seasonRepository;
        private readonly ILogger<CustomerSeasonService> _logger;
        public CustomerSeasonService(IGenericRepository<Season> seasonRepository , ILogger<CustomerSeasonService> logger)
        {
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

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

        public async Task<SeasonDetailsVM?> GetSeasonDetailsAsync(Guid id)
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
                        ThumbnailUrl = !string.IsNullOrEmpty(e.ThumbnailUrl)
                            ? e.ThumbnailUrl
                            : season.TvSeries.ImgUrl, // Fallback to series image
                        //ReleaseDate = e.re
                    }).ToList();

                // Create TV series VM list (if needed for related series)
                var tvSeriesVMs = new List<TvSeriesVM>
        {
            new TvSeriesVM
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

    }
}
