using DAL.ViewModels.Cinema;
using System.Security.Claims;

namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class CinemaReservationsDropdownViewComponent : ViewComponent
    {
        private readonly ApplicationdbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CinemaReservationsDropdownViewComponent(
            ApplicationdbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return View(new List<CinemaReservationVM>());
            }

            var reservations = await _context.CinemaMovies
                .Include(cm => cm.Movie)
                .Include(cm => cm.Cinema)
                .Where(cm => cm.Movie.EndDate >= DateTime.Now)
                .OrderBy(cm => cm.ShowTime)
                .Select(cm => new CinemaReservationVM
                {
                    MovieId = cm.MovieId,
                    MovieTitle = cm.Movie.Title,
                    CinemaName = cm.Cinema.Name,
                    ShowTime = cm.ShowTime,
                    Price = cm.Movie.Price,
                    MovieImage = cm.Movie.ImgUrl
                })
                .ToListAsync();

            return View(reservations);
        }
    }
}
