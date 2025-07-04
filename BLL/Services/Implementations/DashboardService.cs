
namespace BLL.Services.Implementations
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
        private readonly IGenericRepository<Subscriber> _subscriberRepo;
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly IGenericRepository<Order> _orderRepo;

        private readonly IApplicationUserRepository _userRepo;

        public DashboardService(IGenericRepository<Movie> movieRepo,IGenericRepository<TvSeries> tvSeriesRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<Episode> episodeRepo,
            IGenericRepository<Season> seasonRepo,
            IGenericRepository<Subscriber> subscriberRepo,
            IGenericRepository<Special> specialRepo,
            IGenericRepository<Order> orderRepo,
            IApplicationUserRepository userRepo)
        {
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _cinemaRepo = cinemaRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _episodeRepo = episodeRepo;
            _seasonRepo = seasonRepo;
            _subscriberRepo = subscriberRepo;
            _specialRepo = specialRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        #region Admin Methods

        public async Task<AdminDashboardVM> GetAdminDashboardDataAsync()
        {
            return new AdminDashboardVM
            {
                TotalActiveSpecials = await GetTotalActiveSpecialsAsync(),
                TotalCharacters = await GetTotalCharactersAsync(),
                TotalCategories = await GetTotalCategoriesAsync(),
                TotalMovies = await GetTotalMoviesAsync(),
                TotalCinemas = await GetTotalCinemasAsync(),
                TotalTvSeries = await GetTotalTvSeriesAsync(),
                TotalOrders = await GetTotalOrdersAsync(),
                RecentSubscriber = await GetTotalSubscriberAsync(5),
                RecentOrders = await GetRecentOrdersAsync(5),
                TotalUsers = await GetTotalUsersAsync()
            };
        }

        public async Task<int> GetTotalActiveSpecialsAsync()
            => await _specialRepo.GetAll()
                .Where(s => s.StartDate <= DateTime.UtcNow && s.EndDate >= DateTime.UtcNow)
                .CountAsync();

        public async Task<int> GetTotalCharactersAsync()
            => await _characterRepo.GetAll().CountAsync();

        public async Task<int> GetTotalCategoriesAsync()
            => await _categoryRepo.GetAll().CountAsync();

        public async Task<int> GetTotalMoviesAsync()
            => await _movieRepo.GetAll().CountAsync();

        public async Task<int> GetTotalCinemasAsync()
            => await _cinemaRepo.GetAll().CountAsync();

        public async Task<int> GetTotalTvSeriesAsync()
            => await _tvSeriesRepo.GetAll().CountAsync();

        public async Task<int> GetTotalOrdersAsync()
            => await _orderRepo.GetAll().CountAsync();

        public async Task<List<Subscriber>> GetTotalSubscriberAsync(int count)
            => await _subscriberRepo.GetAll()
                .OrderByDescending(s => s.CreatedDateUtc)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetTotalUsersAsync()
            => await _userRepo.GetTotalUsersAsync();

        private async Task<List<Order>> GetRecentOrdersAsync(int count)
    => await _orderRepo.GetAll()
        .OrderByDescending(o => o.OrderDate)
        .Take(count)
        .Include(o => o.ApplicationUser)
        .ToListAsync();


        #endregion

        #region Customer Methods
        public async Task<DashboardVM> GetDashboardDataAsync()
        {
            var now = DateTime.UtcNow;
            var oneMonthAgo = now.AddMonths(-1);
            var cutoffDate = DateTime.UtcNow.AddMonths(-3);

            var dashboardData = new DashboardVM
            {


                // Movies
                RecentMovies = await _movieRepo.Get(m => !m.IsDeleted && m.CurrentState == CurrentState.Active)
                    .OrderByDescending(m => m.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),


                NewReleases = await _movieRepo.Get(m => !m.IsDeleted && m.CreatedDateUtc >= cutoffDate)
                        .OrderByDescending(m => m.Rating)
                        .Take(6)
                        .ToListAsync(),

                TopRated = await _movieRepo.Get(m => !m.IsDeleted && m.CurrentState == CurrentState.Active)
                    .OrderByDescending(m => m.Rating)
                    .Take(6)
                    .ToListAsync(),

                // Cinemas
                RecentCinemas = await _cinemaRepo.Get(c => !c.IsDeleted && c.CurrentState == CurrentState.Active)
                    .OrderByDescending(c => c.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),

                // TV Series
                RecentTvSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState == CurrentState.Active)
                    .OrderByDescending(t => t.CreatedDateUtc)
                    .Take(6)
                    .ToListAsync(),

                PopularSeries = await _tvSeriesRepo.Get(t => !t.IsDeleted && t.CurrentState == CurrentState.Active)
                    .OrderByDescending(t => t.Rating)
                    .Take(6)
                    .ToListAsync(),

                // Categories
                MovieCategories = await _categoryRepo.GetAll().Where(e => !e.IsDeleted && e.CurrentState == CurrentState.Active)
                    .OrderBy(c => c.Name)
                    .Take(8)
                    .ToListAsync()
            };

            return dashboardData;
        }

        #endregion

    }

}
