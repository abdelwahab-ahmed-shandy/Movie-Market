using BLL.Services.Interfaces.Admin;
using DAL.ViewModels.Dashboard;

namespace BLL.Services.Implementations.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGenericRepository<Subscriber> _subscriberRepo;
        private readonly IGenericRepository<Special> _specialRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<TvSeries> _tvSeriesRepo;
        private readonly IGenericRepository<Order> _orderRepo;

        //private readonly IGenericRepository<ApplicationUser> _userRepo;
        public AdminDashboardService(
            IGenericRepository<Subscriber> subscriberRepo,
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<TvSeries> tvSeriesRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Special> specialRepo
            //IGenericRepository<ApplicationUser> userRepo,
            )
        {
            _subscriberRepo = subscriberRepo;
            _movieRepo = movieRepo;
            _tvSeriesRepo = tvSeriesRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _cinemaRepo = cinemaRepo;
            _specialRepo = specialRepo;

            //_userRepo = userRepo;
        }

        public async Task<AdminDashboardVM> GetDashboardDataAsync()
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
                RecentOrders = await GetRecentOrdersAsync(5)
                //TotalUsers = await GetTotalUsersAsync(),
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

        //public async Task<int> GetTotalUsersAsync()
        //    => await _userRepo.GetAll().CountAsync();

        private async Task<List<Order>> GetRecentOrdersAsync(int count)
    => await _orderRepo.GetAll()
        .OrderByDescending(o => o.OrderDate)
        .Take(count)
        .Include(o => o.ApplicationUser)
        .ToListAsync();

    }
}