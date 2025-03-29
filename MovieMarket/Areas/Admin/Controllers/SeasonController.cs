using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMart.Models;

namespace MovieMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly ITvSeriesRepository _tvSeriesRepository;
        public SeasonController(ISeasonRepository seasonRepository, ITvSeriesRepository tvSeriesRepository)
        {
            this._seasonRepository = seasonRepository;
            _tvSeriesRepository = tvSeriesRepository;
        }
        public IActionResult Index(string? query, int page = 1)
        {
            var season = _seasonRepository.Get()
                .Include(s => s.TvSeries)
                .ToList();

            // Check if there is a search query
            if (query != null)
            {
                season = season.Where(e => (e.Title ?? "").Contains(query)
                                         || e.ReleaseYear.ToString().Contains(query)
                                         || e.SeasonNumber.ToString().Contains(query)
                                                                                    ).ToList();
            }

            // Calculate the number of pages required, so that there are 5 customers per page
            int pageSize = 5;
            int totalCustomers = season.Count();
            int totalPages = (int)Math.Ceiling((double)totalCustomers / pageSize);

            // Check if the requested page does not exist
            if (page > totalPages && totalPages > 0)
                return NotFound();

            // Split the customers and display only 5 per page
            season = season.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Pass the number of pages to the View
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;


            return View(season.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.TvSeries = new SelectList(_tvSeriesRepository.Get(), "Id", "Title");
            return View(new Season());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Season season)
        {
            if (ModelState.IsValid)
            {
                _seasonRepository.Create(season);
                _seasonRepository.SaveDB();

                // Set the success message in TempData
                TempData["notifiction"] = "The Season was created successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var season = _seasonRepository.GetOne(s => s.Id == Id);
            if (season != null)
            {
                ViewBag.TvSeries = new SelectList(_tvSeriesRepository.Get(), "Id", "Title", season.TvSeriesId);
                return View(season);
            }
            return RedirectToAction("NotFound", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Season season)
        {
            if (season != null || ModelState.IsValid)
            {
                _seasonRepository.Edit(season);
                _seasonRepository.SaveDB();
                ViewBag.TvSeries = new SelectList(_tvSeriesRepository.Get(), "Id", "Title");

                // Set the success message in TempData
                TempData["notifiction"] = "Edit Season Successfully!";
                TempData["MessageType"] = "Success";

                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }
        public IActionResult Delete(int Id)
        {
            var season = _seasonRepository.GetOne(s => s.Id == Id);

            if (season == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            _seasonRepository.Delete(season);
            _seasonRepository.SaveDB();

            // Set the success message in TempData
            TempData["notifiction"] = "Season Deleted Successfully!";
            TempData["MessageType"] = "Success";

            return RedirectToAction(nameof(Index));
        }
    }
}
