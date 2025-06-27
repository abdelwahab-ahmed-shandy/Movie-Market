using DAL.ViewModels.Movie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Market.GloubalUsing;

namespace Movie_Market.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class MovieController : BaseController
    {
        private readonly IMovieService _movieService;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Character> _characterRepo;
        private readonly IGenericRepository<Cinema> _cinemaRepo;
        private readonly IGenericRepository<Special> _specialRepo;

        public MovieController(
            IMovieService movieService,
            IGenericRepository<Category> categoryRepo,
            IGenericRepository<Character> characterRepo,
            IGenericRepository<Cinema> cinemaRepo,
            IGenericRepository<Special> specialRepo, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _movieService = movieService;
            _categoryRepo = categoryRepo;
            _characterRepo = characterRepo;
            _cinemaRepo = cinemaRepo;
            _specialRepo = specialRepo;
        }


        #region Movie Views

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var movies = await _movieService.GetAllMoviesAsync(pageNumber, pageSize, searchTerm);
            return View(movies);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var movie = await _movieService.GetMovieDetailsAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }


        #endregion

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieAdminCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            var result = await _movieService.CreateMovieAsync(model);
            if (result)
            {

                TempData["notification"] = "Movie created successfully!";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error creating movie");
            await LoadDropdowns();
            return View(model);
        }

        #endregion

        #region Edit 

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var movie = await _movieService.GetMovieForEditAsync(id);
                if (movie == null)
                {
                    TempData["notification"] = "Movie not found";
                    TempData["MessageType"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                await LoadDropdowns();

                ViewBag.SelectedCharacters = movie.CharacterIds;
                ViewBag.SelectedCinemas = movie.CinemaIds;
                ViewBag.SelectedSpecials = movie.SpecialIds;

                return View(movie);
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Error loading movie";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieAdminEditVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdowns();
                    return View(model);
                }

                var result = await _movieService.UpdateMovieAsync(model);
                if (result)
                {
                    TempData["notification"] = "Movie updated successfully!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction(nameof(Index));
                }

                TempData["notification"] = "Error updating movie";
                TempData["MessageType"] = "error";
                ModelState.AddModelError("", "Error updating movie");
                await LoadDropdowns();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["notification"] = "An error occurred while updating the movie";
                TempData["MessageType"] = "error";
                await LoadDropdowns();
                return View(model);
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await _movieService.SoftDeleteMovieAsync(id);
            if (result)
            {
                TempData["notification"] = "Movie soft deleted successfully!";
                TempData["MessageType"] = "Information";
            }
            else
            {
                TempData["notification"] = "Error soft deleting movie";
                TempData["MessageType"] = "Information";
            }
            TempData["notification"] = "Movie Change Status Soft Deleted!";
            TempData["MessageType"] = "Information";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _movieService.RestoreMovieAsync(id);
            if (result)
            {
                TempData["notification"] = "Movie Change Status Soft Deleted!";
                TempData["MessageType"] = "Information";
            }
            else
            {
                TempData["notification"] = "Movie Not Change Status Soft Deleted!";
                TempData["MessageType"] = "Error";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            var result = await _movieService.DeleteMoviePermanentlyAsync(id);
            if (result)
            {
                TempData["notification"] = "Movie permanently deleted!";
                TempData["MessageType"] = "Warning";
            }
            else
            {
                TempData["notification"] = "Movie permanently Not deleted!";
                TempData["MessageType"] = "Error";
            }
            return RedirectToAction(nameof(Index));
        }


        #endregion

        #region Private Methods

        private async Task LoadDropdowns()
        {
            ViewBag.Categories = await _categoryRepo.GetAll()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            ViewBag.Characters = await _characterRepo.GetAll()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            ViewBag.Cinemas = await _cinemaRepo.GetAll()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            ViewBag.Specials = await _specialRepo.GetAll()
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToListAsync();
        }

        #endregion

    }
}
