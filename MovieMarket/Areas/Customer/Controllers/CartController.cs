using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMarket.Repositories.IRepositories;
using MovieMart.Models;
using System;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ICinemaRepository _cinemaRepository;
        public CartController(UserManager<ApplicationUser> userManager, ICartRepository cartRepository,
                                    IMovieRepository movieRepository, ICinemaRepository cinemaRepository)
        {
            this._userManager = userManager;
            this._cartRepository = cartRepository;
            this._cinemaRepository = cinemaRepository;
            this._movieRepository = movieRepository;
        }
        public IActionResult Index()
        {
            var appUserId = _userManager.GetUserId(User);

            var cart = _cartRepository.Get(e => e.ApplicationUserId == appUserId, includes: [e => e.Movie, e => e.Cinema, e => e.ApplicationUser]);

            ViewBag.Total = cart.Sum(e => e.Movie.Price * e.Count);


            return View(cart.ToList());
        }

        #region Add And Delete Cart
        public IActionResult AddToCart(int MovieId, int CinemaId, int count)
        {
            var appUserId = _userManager.GetUserId(User);


            //todo : Error in The Valadate ...

            /* Validate the entered data
             If the user ID is empty (meaning the user is not logged in),
             If the movie ID (MovieId) or cinema ID (CinemaId) is zero or less (invalid information), or
             If the number of tickets (count) is less than or equal to zero (invalid information),
             A BadRequest status will be returned to indicate that there is an error in the entered data */
            //if (string.IsNullOrEmpty(appUserId) || MovieId <= 0 || CinemaId <= 0 || count <= 0)
            //{
            //    TempData["ErrorMessage"] = "Some of the input data is invalid.";
            //    return RedirectToAction("ErrorPage");  // أو أي صفحة تعرض الأخطاء
            //}

            Cart cart = new Cart()
            {
                ApplicationUserId = appUserId,
                MovieId = MovieId,
                CinemaId = CinemaId,
                Count = count
            };

            var cartDB = _cartRepository.GetOne(e => e.ApplicationUserId == appUserId &&
                                                 e.MovieId == MovieId && e.CinemaId == CinemaId);

            if (cartDB != null)
                cartDB.Count = cartDB.Count + count;
            else
                _cartRepository.Create(cart);

            if (cartDB != null)
            {
                cartDB.Count += count;
            }
            else
            {
                _cartRepository.Create(cart);
            }

            _cartRepository.SaveDB();

            TempData["notifiction"] = "Add Movie To Cart successfully!";
            TempData["MessageType"] = "Success";

            return RedirectToAction("Index", "Cart", new { area = "Customer" });
        }

        public IActionResult DeleteOnCart()
        {
            var appUserId = _userManager.GetUserId(User);

            var cartItems = _cartRepository.Get(e => e.ApplicationUserId == appUserId).ToList();

            if (cartItems.Any())
            {
                foreach (var item in cartItems)
                {
                    _cartRepository.Delete(item);
                    _cartRepository.SaveDB();
                }

                TempData["notifiction"] = "🧹 Your cart has been emptied successfully.";
                TempData["MessageType"] = "Success";
            }
            else
            {
                TempData["notifiction"] = "🛒 Your cart is already empty.";
                TempData["MessageType"] = "Warning";
            }


            return RedirectToAction("Index");
        }

        #endregion

        #region Increment And Decrement
        public IActionResult Increment(int movieId)
        {
            var appUserId = _userManager.GetUserId(User);

            var cartItem = _cartRepository.GetOne(e => e.ApplicationUserId == appUserId && e.MovieId == movieId); // مثال لتحميل العنصر من قاعدة البيانات
            if (cartItem != null)
            {
                cartItem.Count += 1;
                _cartRepository.SaveDB();
            }

            TempData["notifiction"] = " One More Ticket Added To Your Cart !";
            TempData["MessageType"] = "Success";
            return RedirectToAction("Index");
        }

        public IActionResult Decrement(int movieId)
        {
            var appUserId = _userManager.GetUserId(User);

            var cartItem = _cartRepository.GetOne(e => e.ApplicationUserId == appUserId && e.MovieId == movieId);

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count -= 1;
                    _cartRepository.SaveDB();
                    TempData["notifiction"] = "🛒 One Ticket Removed From Your Cart!";
                }
                else if (cartItem.Count == 1)
                {
                    _cartRepository.Delete(cartItem);
                    _cartRepository.SaveDB();
                    TempData["notifiction"] = "🗑️ Movie Removed From Your Cart.";
                }

                TempData["MessageType"] = "Success";
            }
            else
            {
                TempData["notifiction"] = "Movie not found in the cart.";
                TempData["MessageType"] = "Error";
            }

            return RedirectToAction("Index");
        }
        #endregion

        // todo : GO Head Create a Pay ....
    }
}
