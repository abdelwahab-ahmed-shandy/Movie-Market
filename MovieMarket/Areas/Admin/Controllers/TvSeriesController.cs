using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMart.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MovieMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class TvSeriesController : Controller
    {
        private readonly ITvSeriesRepository _tvSeriesRepository;
        public TvSeriesController(ITvSeriesRepository tvSeriesRepository)
        {
            this._tvSeriesRepository = tvSeriesRepository;
        }
        public IActionResult Index(string? query, int page = 1)
        {
            var tvSeries = _tvSeriesRepository.Get().ToList();

            if (query != null)
            {
                tvSeries = tvSeries.Where(e => (e.Title ?? "").Contains(query)
                || (e.Description ?? "").Contains(query)
                || (e.Author ?? "").Contains(query)
                || (e.ImgUrl ?? "").Contains(query)
                || (e.ReleaseYear.ToString().Contains(query))
                || (e.Rating.HasValue ? e.Rating.Value.ToString("0") : "").Contains(query)
                ).ToList();
            }

            int pageSize = 5;
            int totalCustomers = tvSeries.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            if (page > totalPages && totalPages > 0)
                return NotFound();

            tvSeries = tvSeries.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            return View(tvSeries.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TvSeries());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TvSeries tvSeries, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\TvSeries", fileName);

                    using (var steam = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(steam);
                    }
                    tvSeries.ImgUrl = fileName;
                }
                _tvSeriesRepository.Create(tvSeries);
                _tvSeriesRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "The Tv-Series was created successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction("Index");
            }
            return View(tvSeries);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            // Retrieve the movie from the repository using the given Id
            var tvSeries = _tvSeriesRepository.GetOne(m => m.Id == Id);

            // Check if the movie exists
            if (tvSeries != null)
            {
                // Populate ViewBag with category options for the dropdown list
                ViewBag.Categories = new SelectList(_tvSeriesRepository.Get(), "Id", "Name", tvSeries.Id);

                // Return the edit view with the movie data
                return View(tvSeries);
            }
            // If the movie is not found, redirect to the "NotFound" page
            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TvSeries tvSeries, IFormFile? file)
        {
            // Retrieve the existing movie from the database without tracking changes
            var tvSeriesInDB = _tvSeriesRepository.GetOne(e => e.Id == tvSeries.Id, tracked: false);

            // If the movie exists and a new file is uploaded, update the image
            if (tvSeriesInDB != null && file != null && file.Length > 0)
            {
                // Generate a unique file name and determine the file path
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\TvSeries", fileName);

                // Save the new file
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                // Delete the old image file if it exists
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\TvSeries", tvSeriesInDB.ImgUrl);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Update the movie's image URL
                tvSeries.ImgUrl = fileName;
            }
            else
            {
                // If no new file is uploaded, retain the existing image
                tvSeries.ImgUrl = tvSeriesInDB.ImgUrl;
            }

            // If the movie exists, update it in the database
            if (tvSeries != null)
            {
                _tvSeriesRepository.Edit(tvSeries);
                _tvSeriesRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "Edit Tv-Series Successfully!";
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
            var movie = _tvSeriesRepository.GetOne(e => e.Id == Id);

            if (movie != null)
            {
                // Determine the file path of the current image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\TvSeries", movie.ImgUrl);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Remove the image reference from the movie entity
                movie.ImgUrl = null;
                _tvSeriesRepository.SaveDB();

                // Redirect to the Edit page after deleting the image
                return RedirectToAction("Edit", new { Id });
            }
            // Redirect to a "Not Found" page if the movie does not exist
            return RedirectToAction("NotFound", "Home");
        }

        public IActionResult Delete(int Id)
        {
            // Retrieve the movie from the database
            var movie = _tvSeriesRepository.GetOne(m => m.Id == Id);

            if (movie.ImgUrl != null)
            {
                // Determine the file path of the movie's image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Assets\\Customer\\TvSeries", movie.ImgUrl);

                // Delete the image file if it exists
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete the movie from the database
                _tvSeriesRepository.Delete(movie);
                _tvSeriesRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "Delete Tv-Series Successfully!";
                TempData["MessageType"] = "Success";

                // Redirect to the list of movies after deletion
                return RedirectToAction(nameof(Index));
            }
            // Redirect to a "Not Found" page if the movie does not exist
            return RedirectToAction("NotFound", "Home");
        }
    }
}
