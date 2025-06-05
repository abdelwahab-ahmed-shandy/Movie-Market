using DAL.ViewModels.Cart;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Movie_Market.Areas.Customer.Views.Shared.Components
{
    public class CartDropdownViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartDropdownViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var cartItems = _cartService.GetCartItems();
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
