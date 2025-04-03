using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMarket.Models;
using MovieMarket.Models.ViewModels;
using MovieMarket.Repositories.IRepositories;
using MovieMart.Models;

namespace MovieMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuberAdmin")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IRepository<CinemaMovie> _cinemaMovieRepository;

        public CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository, IRepository<CinemaMovie> cinemaMovieRepository)
        {
            this._cinemaRepository = cinemaRepository;
            this._movieRepository = movieRepository;
            this._cinemaMovieRepository = cinemaMovieRepository;
        }
        #region View All Data :
        public IActionResult Index(string? query, int page = 1)
        {
            var cinema = _cinemaRepository.Get();

            int pageSize = 5;
            int totalCinema = cinema.Count();
            int totalPages = (int)Math.Ceiling((double)totalCinema / pageSize);

            if (page > totalPages && totalPages > 0)
                return NotFound();

            cinema = cinema.Skip((page - 1) * pageSize).Take(pageSize);
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            if (query != null)
            {
                cinema = cinema.Where(c => c.Name.Contains(query)
                                        || c.Description.Contains(query)
                                        || c.Location.Contains(query)
                                     );
            }
            return View(cinema.ToList());
        }

        #endregion

        #region Create :
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var movies = await _movieRepository.Get().ToListAsync();
            ViewBag.Movies = new SelectList(movies, "Id", "Title");

            return View(new CinemaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaViewModel cinemaViewModel)
        {
            if (ModelState.IsValid)
            {
                var cinemaDB = new Cinema
                {
                    Name = cinemaViewModel.Name,
                    Description = cinemaViewModel.Description,
                    Location = cinemaViewModel.Location,

                    CinemaMovies = cinemaViewModel.SelectedMovieIds.Select(movieId => new CinemaMovie
                    {
                        MovieId = movieId
                    }).ToList()

                };


                _cinemaRepository.Create(cinemaDB);
                _cinemaRepository.SaveDB();

                TempData["notifiction"] = "The Cinema was created successfully!";
                TempData["MessageType"] = "success";

                return RedirectToAction("Index");

            }

            ViewBag.Movies = new MultiSelectList(await _movieRepository.Get().ToListAsync(), "Id", "Title");
            return View(cinemaViewModel);
        }
        #endregion

        #region Edit :

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var cinema = _cinemaRepository.Get()
                .Include(c => c.CinemaMovies)
                .ThenInclude(cm => cm.Movie)
                .FirstOrDefault(c => c.Id == Id);

            if (cinema != null)
            {
                var selectedMovies = cinema.CinemaMovies.Select(cm => cm.MovieId).ToList();

                var cinemaView = new CinemaViewModel
                {
                    Name = cinema.Name,
                    Description = cinema.Description,
                    Location = cinema.Location,
                    SelectedMovieIds = selectedMovies
                };

                ViewBag.Movies = _movieRepository.Get()
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Title,
                        Selected = selectedMovies.Contains(m.Id)
                    })
                    .ToList();

                return View(cinemaView);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(CinemaViewModel cinemaViewModel)
        {
            var cinema = _cinemaRepository.Get()
                                          .Include(c => c.CinemaMovies)
                                          .FirstOrDefault(c => c.Id == cinemaViewModel.Id);

            if (cinema != null)
            {
                cinema.Name = cinemaViewModel.Name;
                cinema.Description = cinemaViewModel.Description;
                cinema.Location = cinemaViewModel.Location;

                cinema.CinemaMovies.Clear();
                if (cinemaViewModel.SelectedMovieIds != null)
                {
                    foreach (var movieId in cinemaViewModel.SelectedMovieIds)
                    {
                        cinema.CinemaMovies.Add(new CinemaMovie
                        {
                            CinemaId = cinema.Id,
                            MovieId = movieId
                        });
                    }
                }

                _cinemaRepository.Edit(cinema);
                _cinemaRepository.SaveDB();

                TempData["notifiction"] = "Edit Cinema Successfully!";
                TempData["MessageType"] = "Success";

                return RedirectToAction("Index");

            }

            return NotFound();
        }

        #endregion

        #region Delete :

        public IActionResult Delete(int Id)
        {
            var cinema = _cinemaRepository.Get()
                .Include(c => c.CinemaMovies)
                .ThenInclude(cm => cm.Movie)
                .FirstOrDefault(c => c.Id == Id);

            if (cinema != null)
            {
                // Delete all relationships between cinema and films
                _cinemaMovieRepository.DeleteAll(cinema.CinemaMovies.ToList());
                _cinemaMovieRepository.SaveDB();

                _cinemaRepository.Delete(cinema);
                _cinemaRepository.SaveDB();

                TempData["notifiction"] = "Cinema Deleted Successfully!";
                TempData["MessageType"] = "Success";

                return RedirectToAction("Index");
            }

            return NotFound();
        }

        #endregion

    }
}
