namespace Movie_Market.Areas.Customer.ViewComponents
{
    public class CartDropdownViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartDropdownViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItems = await _cartService.GetCartItemsAsync();
            var total = _cartService.CalculateTotal();

            var model = new CartDropdownVM
            {
                Items = cartItems,
                Total = total
            };

            return View(model);
        }
    }
}
