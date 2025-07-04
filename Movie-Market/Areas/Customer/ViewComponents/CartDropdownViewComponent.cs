namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class CartDropdownViewComponent : ViewComponent
    {
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartDropdownViewComponent(
            IGenericRepository<Cart> cartRepository,
            UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            if (string.IsNullOrEmpty(userId))
            {
                return View(new CartDropdownVM { Items = new List<CartItemVM>(), Total = 0 });
            }

            // Use Get() instead of Find() for composite key entities
            var cartItems = await _cartRepository.Get(c =>
                    c.ApplicationUserId == Guid.Parse(userId) &&
                    c.CartStatus == CartStatus.Active)
                .Include(c => c.Movie)
                .Include(c => c.Cinema)
                .ToListAsync();

            var items = cartItems.Select(item => new CartItemVM
            {
                Id = item.Id, // Note: This might need adjustment since Cart has composite key
                MovieId = item.MovieId,
                Title = item.Movie.Title,
                ImageUrl = item.Movie.ImgUrl,
                CinemaName = item.Cinema.Name,
                Price = item.Movie.Price,
                Quantity = item.Count
            }).ToList();

            var total = cartItems.Sum(item => item.Movie.Price * item.Count);

            return View(new CartDropdownVM
            {
                Items = items,
                Total = total,
                ItemCount = cartItems.Sum(item => item.Count)
            });
        }
   
    
    }
}
