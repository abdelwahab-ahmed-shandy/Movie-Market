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
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Episode> _episodeRepo;
        private readonly IGenericRepository<Season> _seasonRepo;

        public DashboardService(
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<TvSeries> tvSeriesRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<Episode> episodeRepo,
            IGenericRepository<Season> seasonRepo)
        {
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _cinemaRepo = cinemaRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _episodeRepo = episodeRepo;
            _seasonRepo = seasonRepo;
        }

        public async Task<DashboardVM> GetDashboardDataAsync()
        {
            var now = DateTime.UtcNow;
            var oneMonthAgo = now.AddMonths(-1);
            var cutoffDate = DateTime.UtcNow.AddMonths(-3);

            var dashboardData = new DashboardVM
            {


                // Movies
                RecentMovies = await _movieRepo.Get(m => !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),


                NewReleases = await _movieRepo.Get(m => !m.IsDeleted && m.CreatedDateUtc >= cutoffDate)
                        .OrderByDescending(m => m.Rating)
                        .Take(6)
                        .ToListAsync(),

                TopRated = await _movieRepo.Get(m => !m.IsDeleted)
                    .OrderByDescending(m => m.Rating)
                    .Take(6)
                    .ToListAsync(),

                // Cinemas
                RecentCinemas = await _cinemaRepo.Get(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),

                // TV Series
                RecentTvSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),

                PopularSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted)
                    .OrderByDescending(t => t.Rating)
                    .Take(6)
                    .ToListAsync(),

                // Categories
                MovieCategories = await _categoryRepo.GetAll()
                    .OrderBy(c => c.Name)
                    .Take(8)
                    .ToListAsync()
            };

            return dashboardData;
        }
    }

}
