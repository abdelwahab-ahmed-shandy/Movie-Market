using DAL.ViewModels.Season;
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

        public CustomerSeasonService(IGenericRepository<Season> seasonRepository)
        {
            _seasonRepository = seasonRepository;
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
            var season = await _seasonRepository.GetAll()
                .Include(s => s.TvSeries)
                .Include(s => s.Episodes.Where(e => !e.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id);

            if (season == null)
            {
                return null;
            }

            var episodes = season.Episodes
                .OrderBy(e => e.EpisodeNumber)
                .Select(e => new EpisodeCustomerVM
                {
                    Id = e.Id,
                    Title = e.Title,
                    EpisodeNumber = e.EpisodeNumber,
                    Description = e.Description,
                    Duration = e.Duration,
                    ThumbnailUrl = e.ThumbnailUrl,
                    //ReleaseDate = e.ReleaseDate
                }).ToList();

            return new SeasonDetailsVM
            {
                Id = season.Id,
                Title = season.Title,
                SeasonNumber = season.SeasonNumber,
                ReleaseYear = season.ReleaseYear,
                //Description = season.Description,
                TvSeriesTitle = season.TvSeries.Title,
                TvSeriesId = season.TvSeries.Id,
                ImgUrl = season.TvSeries.ImgUrl,
                Episodes = episodes
            };
        }

    }
}
