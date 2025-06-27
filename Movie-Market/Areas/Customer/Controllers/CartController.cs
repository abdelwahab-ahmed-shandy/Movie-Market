using DAL.Repositories;
using DAL.Repositories.IRepositories;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        private readonly IGenericRepository<Movie> _movieRepo;

        public CartController(ICartService cartService, IGenericRepository<Movie> movieRepo, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _cartService = cartService;
            _movieRepo = movieRepo;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid movieId, int quantity = 1)
        {
            var movie = await _movieRepo.GetByIdAsync(movieId);
            if (movie == null) return NotFound();

            _cartService.AddToCart(movie, quantity);
            return RedirectToAction("Index", "Movies");
        }


        [HttpPost]
        public IActionResult RemoveFromCart(Guid movieId)
        {
            _cartService.RemoveFromCart(movieId);
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }
    }
}
