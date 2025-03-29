using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;
        public MovieController(IMovieRepository movieRepository, ICategoryRepository categoryRepository)
        {
            this._movieRepository = movieRepository;
            this._categoryRepository = categoryRepository;
        }
        public IActionResult Index(string? query, int page = 1)
        {
            var movie = _movieRepository.Get()
                .Include(c => c.Category)
                .ToList();

            if (query != null)
            {
                movie = movie.Where(e => (e.Title ?? "").Contains(query)
                || (e.Description ?? "").Contains(query)
                || (e.Category.Name ?? "").Contains(query)
                || (e.Author ?? "").Contains(query)
                || e.Price.ToString("0").Contains(query)
                || (e.StartDate?.ToString("yyyy-MM-dd") ?? "").Contains(query)
                || (e.EndDate?.ToString("yyyy-MM-dd") ?? "").Contains(query)
                || (e.Duration is TimeSpan ts ? ts.ToString(@"hh\:mm\:ss") : "").Contains(query)
                || (e.Rating.HasValue ? e.Rating.Value.ToString("0") : "").Contains(query)
                ).ToList();
            }

            int pageSize = 5;
            int totalCustomers = movie.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            if (page > totalPages && totalPages > 0)
                return NotFound();

            movie = movie.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            return View(movie);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Retrieve the list of categories from the repository and convert it to a SelectList
            // This list will be used to populate the category dropdown in the view
            ViewBag.Categories = new SelectList(_categoryRepository.Get(), "Id", "Name");

            // Return the Create view with an empty Movie object
            return View(new Movie());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile? file)
        {
            // Check if the model state is valid (i.e., all required fields are correctly filled)
            if (ModelState.IsValid)
            {
                // Check if a file is uploaded
                if (file != null && file.Length > 0)
                {
                    // Generate a unique file name using GUID and retain the original file extension
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Define the file path where the image will be stored in the wwwroot directory
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\MoviesPhoto", fileName);

                    // Save the uploaded file to the specified location
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    // Assign the file name to the movie object to store in the database
                    movie.ImgUrl = fileName;
                }
                // Save the movie details in the database
                _movieRepository.Create(movie);
                _movieRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "The Movie was created successfully!";
                TempData["MessageType"] = "success";

                // Redirect the user to the movie list page after successful creation
                return RedirectToAction("Index");
            }

            // If the model state is invalid, reload the category dropdown and return the same view
            ViewBag.Categories = new SelectList(_categoryRepository.Get(), "Id", "Name");
            return View(movie);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            // Retrieve the movie from the repository using the given Id
            var movie = _movieRepository.GetOne(m => m.Id == Id);

            // Check if the movie exists
            if (movie != null)
            {
                // Populate ViewBag with category options for the dropdown list
                ViewBag.Categories = new SelectList(_categoryRepository.Get(), "Id", "Name", movie.CategoryId);

                // Return the edit view with the movie data
                return View(movie);
            }
            // If the movie is not found, redirect to the "NotFound" page
            return RedirectToAction("NotFound", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile? file)
        {
            // Retrieve the existing movie from the database without tracking changes
            var movieInDB = _movieRepository.GetOne(e => e.Id == movie.Id, tracked: false);

            // If the movie exists and a new file is uploaded, update the image
            if (movieInDB != null && file != null && file.Length > 0)
            {
                // Generate a unique file name and determine the file path
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\MoviesPhoto", fileName);

                // Save the new file
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                // Delete the old image file if it exists
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\MoviesPhoto", movieInDB.ImgUrl);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Update the movie's image URL
                movie.ImgUrl = fileName;
            }
            else
            {
                // If no new file is uploaded, retain the existing image
                movie.ImgUrl = movieInDB.ImgUrl;
            }

            // If the movie exists, update it in the database
            if (movie != null)
            {
                _movieRepository.Edit(movie);
                _movieRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "Edit Movie Successfully!";
                TempData["MessageType"] = "Success";

                // Redirect to the list of movies after editing
                return RedirectToAction("Index");
            }
            // Redirect to a "Not Found" page if the movie does not exist
            return RedirectToAction("NotFound", "Home");
        }


        public IActionResult DeleteImg(int Id)
        {
            // Retrieve the movie from the database
            var movie = _movieRepository.GetOne(e => e.Id == Id);

            if (movie != null)
            {
                // Determine the file path of the current image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\MoviesPhoto", movie.ImgUrl);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Remove the image reference from the movie entity
                movie.ImgUrl = null;
                _movieRepository.SaveDB();

                // Redirect to the Edit page after deleting the image
                return RedirectToAction("Edit", new { Id });
            }
            // Redirect to a "Not Found" page if the movie does not exist
            return RedirectToAction("NotFound", "Home");
        }

        public IActionResult Delete(int Id)
        {
            // Retrieve the movie from the database
            var movie = _movieRepository.GetOne(m => m.Id == Id);

            if (movie.ImgUrl != null)
            {
                // Determine the file path of the movie's image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\MoviesPhoto", movie.ImgUrl);

                // Delete the image file if it exists
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete the movie from the database
                _movieRepository.Delete(movie);
                _movieRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "Movie Deleted Successfully!";
                TempData["MessageType"] = "Success";

                // Redirect to the list of movies after deletion
                return RedirectToAction(nameof(Index));
            }
            // Redirect to a "Not Found" page if the movie does not exist
            return RedirectToAction("NotFound", "Home");
        }

    }
}
