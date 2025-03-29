using Microsoft.AspNetCore.Mvc;
using MovieMart.Repositories.IRepositories;

namespace MovieMart.Areas.Users.Views.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMovieRepository movieRepository;
        public CategoryController(ICategoryRepository categoryRepository, IMovieRepository movieRepository)
        {
            this.categoryRepository = categoryRepository;
            this.movieRepository = movieRepository;
        }
        public IActionResult Index()
        {
            var ViewCaregory = categoryRepository.Get();
            return View(ViewCaregory.ToList());
        }

        public IActionResult MoreMovieWithCategory(int Id)
        {
            var ViewCaregory = movieRepository.Get()
                .Where(m => m.CategoryId == Id)
                .Include(m => m.Category)
                .ToList();

            if (ViewCaregory == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(ViewCaregory);
        }
    }
}
