using DAL.Repositories;
using DAL.Repositories.IRepositories;
using DAL.ViewModels.Cart;
using Movie_Market.GloubalUsing;
using System.Security.Claims;

namespace Movie_Market.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;
        private readonly IGenericRepository<Movie> _movieRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Cart> _cartRepo;

        public CartController(
            ICartService cartService,
            IGenericRepository<Movie> movieRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Cart> cartRepo,
            UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _cartService = cartService;
            _movieRepo = movieRepo;
            _cinemaRepo = cinemaRepo;
            _cartRepo = cartRepo;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid movieId, Guid cinemaId, int quantity = 1)
        {
            try
            {
                var movie = await _movieRepo.GetByIdAsync(movieId);
                var cinema = await _cinemaRepo.GetByIdAsync(cinemaId);

                if (movie == null || cinema == null)
                {
                    TempData["notification"] = "Movie or cinema not found";
                    TempData["MessageType"] = "Error";

                    return RedirectToAction("Details", "Movie", new { id = movieId });
                }

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Check if already in cart
                var existingItem = await _cartRepo.GetOneAsync(c =>
                    c.ApplicationUserId == userId &&
                    c.MovieId == movieId &&
                    c.CinemaId == cinemaId &&
                    c.CartStatus == CartStatus.Active);

                if (existingItem != null)
                {
                    existingItem.Count += quantity;
                    await _cartRepo.Update(existingItem);

                    TempData["notification"] = "Updated quantity for {movie.Title} at {cinema.Name}";
                    TempData["MessageType"] = "success";

                }
                else
                {
                    var newCartItem = new Cart
                    {
                        ApplicationUserId = userId,
                        MovieId = movieId,
                        CinemaId = cinemaId,
                        Count = quantity,
                        CartStatus = CartStatus.Active,
                        CreatedDateUtc = DateTime.UtcNow
                    };
                    await _cartRepo.Add(newCartItem);

                    TempData["notification"] = $"{movie.Title} added to cart for {cinema.Name}";
                    TempData["MessageType"] = "success";
                }

                return RedirectToAction("Details", "Movie", new { id = movieId });
            }
            catch (Exception ex)
            {

                TempData["notification"] = "An error occurred while adding to cart";
                TempData["MessageType"] = "Error";

                return RedirectToAction("Details", "Movie", new { id = movieId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid cartId, int change)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItem = await _cartRepo.GetOneAsync(c =>
                c.Id == cartId &&
                c.ApplicationUserId == userId);

            if (cartItem != null)
            {
                var newQuantity = cartItem.Count + change;
                if (newQuantity > 0)
                {
                    cartItem.Count = newQuantity;

                    await _cartRepo.Update(cartItem);

                    TempData["notification"] = "Quantity updated successfully.";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    await _cartRepo.DeleteInDB(cartId);
                    TempData["notification"] = "Item removed from cart.";
                    TempData["MessageType"] = "Information";
                }

            }

            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> ViewCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cartItems = await _cartRepo.Get(c =>
                    c.ApplicationUserId == userId &&
                    c.CartStatus == CartStatus.Active) 
                .Include(c => c.Movie)
                .Include(c => c.Cinema)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _cartRepo.Get(c =>
                c.ApplicationUserId == userId &&
                c.CartStatus == CartStatus.Active)
                .SumAsync(c => c.Count);

            return Json(new { count });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(Guid cartId)
        {
            var result = await _cartService.RemoveFromCartAsync(cartId);
            if (result)
            {
                TempData["notification"] = "Item removed successfully";
                TempData["MessageType"] = "success";
            }
            else
            {

                TempData["notification"] = "Item not found or already removed";
                TempData["MessageType"] = "Error";
            }
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync();
            if (result)
            {

                TempData["notification"] = "Cart cleared successfully";
                TempData["MessageType"] = "success";

            }
            else
            {

                TempData["notification"] = "An error occurred while clearing cart";
                TempData["MessageType"] = "Error";
            }
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCartAjax(Guid cartId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Json(new { success = false, message = "Unauthorized" });

                var result = await _cartService.RemoveFromCartAsync(cartId);
                if (result)
                {
                    var count = await _cartRepo.Get(c =>
                        c.ApplicationUserId == userId.Value &&
                        c.CartStatus == CartStatus.Active)
                        .SumAsync(c => c.Count);

                    return Json(new
                    {
                        success = true,
                        message = "Item removed",
                        count
                    });
                }

                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred" });
            }
        }


    }
}
