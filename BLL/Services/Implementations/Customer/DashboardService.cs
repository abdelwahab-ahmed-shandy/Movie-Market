using DAL.Repositories;
using DAL.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations.Customer
{
    public class DashboardService : IDashboardService
    {
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Episode> _episodeRepo;
        private readonly IGenericRepository<Season> _seasonRepo;

        public DashboardService(IGenericRepository<Movie> movieRepo,IGenericRepository<Cinema> cinemaRepo,
                                    IGenericRepository<TvSeries> tvSeriesRepo,
                                        IGenericRepository<Category> categoryRepo,
                                            IGenericRepository<Character> characterRepo,
                                                IGenericRepository<Episode> episodeRepo,
                                                    IGenericRepository<Season> seasonRepo)
        {
            _movieRepo = movieRepo;
            _cinemaRepo = cinemaRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _episodeRepo = episodeRepo;
            _seasonRepo = seasonRepo;
        }

        public async Task<DashboardVM> GetDashboardDataAsync()
        {
            return new DashboardVM
            {
                TotalMovies = await _movieRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalCinemas = await _cinemaRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalTvSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalCategories = await _categoryRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalCharacters = await _characterRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalEpisodes = await _episodeRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),
                TotalSeasons = await _seasonRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active).CountAsync(),

                RecentMovies = await _movieRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .OrderByDescending(m => m.CreatedDateUtc)
                    .Take(5)
                    .ToListAsync(),

                NewReleases = await _movieRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .OrderByDescending(m => m.ReleaseYear)
                    .Take(6)
                    .ToListAsync(),

                TopRated = await _movieRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .OrderByDescending(m => m.Rating)
                    .Take(6)
                    .ToListAsync(),

                RecentCinemas = await _cinemaRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .OrderByDescending(c => c.CreatedDateUtc)
                    .Take(5)
                    .ToListAsync(),

                RecentTvSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .OrderByDescending(t => t.CreatedDateUtc)
                    .Take(5)
                    .ToListAsync(),

                PopularSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .Include(s => s.Seasons)
                    .OrderByDescending(s => s.ReleaseYear)
                    .Take(6)
                    .ToListAsync(),

                MovieCategories = await _categoryRepo.Get(t => !t.IsDeleted && t.CurrentState.Value == CurrentState.Active)
                    .Take(8)
                    .ToListAsync()
            };
        }


    }
}
