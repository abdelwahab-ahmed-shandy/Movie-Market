using BLL.Services.Interfaces.Customer;
using DAL.ViewModels.Character;
using DAL.ViewModels.Season;
using DAL.ViewModels.TvSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Customer
{

    public class CustomerTvSeriesService : ICustomerTvSeriesService
    {
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IGenericRepository<Season> _seasonRepo;
        private readonly IGenericRepository<Character> _characterRepo;

        public CustomerTvSeriesService(
            IGenericRepository<TvSeries> tvSeriesRepo,
            IGenericRepository<Season> seasonRepo,
            IGenericRepository<Character> characterRepo)
        {
            _tvSeriesRepo = tvSeriesRepo;
            _seasonRepo = seasonRepo;
            _characterRepo = characterRepo;
        }

        public async Task<IEnumerable<TvSeriesVM>> GetAllTvSeriesAsync()
        {
            var tvSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                .OrderByDescending(t => t.ReleaseYear)
                .ToListAsync();

            return tvSeries.Select(t => new TvSeriesVM
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Author = t.Author,
                ImgUrl = t.ImgUrl,
                ReleaseYear = t.ReleaseYear,
                Rating = t.Rating
            });
        }



        public async Task<TvSeriesDetailsVM?> GetTvSeriesWithDetailsAsync(Guid id)
        {
            var tvSeries = await _tvSeriesRepo.Get(t => t.Id == id)
                .Include(t => t.Seasons)
                .Include(t => t.Characters)
                .ThenInclude(ct => ct.Character)
                .FirstOrDefaultAsync();

            if (tvSeries == null) return null;

            return new TvSeriesDetailsVM
            {
                TvSeries = new TvSeriesVM
                {
                    Id = tvSeries.Id,
                    Title = tvSeries.Title,
                    Description = tvSeries.Description,
                    Author = tvSeries.Author,
                    ImgUrl = tvSeries.ImgUrl,
                    ReleaseYear = tvSeries.ReleaseYear,
                    Rating = tvSeries.Rating
                },
                Seasons = tvSeries.Seasons
                    .OrderBy(s => s.SeasonNumber)
                    .Select(s => new SeasonVM
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ReleaseYear = s.ReleaseYear,
                        SeasonNumber = s.SeasonNumber
                    }).ToList(),
                Characters = tvSeries.Characters
                    .Select(ct => new CharacterVM
                    {
                        Id = ct.Character.Id,
                        Name = ct.Character.Name,
                        Description = ct.Character.Description
                    }).ToList()
            };
        }



    }
}
